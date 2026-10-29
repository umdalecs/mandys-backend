using Amazon.S3;
using Amazon.S3.Model;
using Mandys.Application.Configuration;
using Mandys.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Mandys.Infrastructure.Services;

public class S3StorageService(IAmazonS3 s3Client, IOptions<S3Options> s3Options) : IStorageService
{
    private readonly S3Options _options = s3Options.Value;

    public async Task<string> UploadAsync(
        Stream content,
        string key,
        string contentType,
        CancellationToken ct = default)
    {
        var normalizedKey = NormalizeKey(key);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = normalizedKey,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await s3Client.PutObjectAsync(request, ct);
        return GetPublicUrl(normalizedKey);
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
    {
        var normalizedKey = NormalizeKey(key);
        var response = await s3Client.GetObjectAsync(_options.BucketName, normalizedKey, ct);
        return response.ResponseStream;
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var normalizedKey = NormalizeKey(key);
        await s3Client.DeleteObjectAsync(_options.BucketName, normalizedKey, ct);
    }

    public string GetPublicUrl(string key)
    {
        var normalizedKey = NormalizeKey(key);
        var baseUrl = (!string.IsNullOrWhiteSpace(_options.PublicUrl) ? _options.PublicUrl : _options.Endpoint)
            .TrimEnd('/');

        return _options.ForcePathStyle
            ? $"{baseUrl}/{_options.BucketName}/{normalizedKey}"
            : $"{baseUrl}/{normalizedKey}";
    }

    private static string NormalizeKey(string key)
        => key.Trim().TrimStart('/');
}
