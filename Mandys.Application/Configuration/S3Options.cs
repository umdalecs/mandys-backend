namespace Mandys.Application.Configuration;

/// <summary>
/// S3-compatible object storage settings (RustFS in local development,
/// AWS S3 / Cloudflare R2 in production), bound from the "S3" section.
/// </summary>
public class S3Options
{
    public const string SectionName = "S3";

    public string Endpoint { get; set; } = "http://localhost:9000";
    public string PublicUrl { get; set; } = "http://localhost:9000";
    public string Region { get; set; } = "us-east-1";
    public string BucketName { get; set; } = "mandys";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool ForcePathStyle { get; set; } = true;
}
