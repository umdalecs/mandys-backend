namespace Mandys.Application.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(
        Stream content,
        string key,
        string contentType,
        CancellationToken ct = default);

    Task<Stream> DownloadAsync(
        string key,
        CancellationToken ct = default);

    Task DeleteAsync(
        string key,
        CancellationToken ct = default);

    string GetPublicUrl(string key);
}
