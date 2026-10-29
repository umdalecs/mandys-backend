using Amazon.Runtime;
using Amazon.S3;
using Mandys.Application.Configuration;
using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Mandys.Api.Configuration;

public static class StorageConfiguration
{
    public static void ConfigureStorage(this WebApplicationBuilder wab)
    {
        wab.Services.Configure<S3Options>(wab.Configuration.GetSection(S3Options.SectionName));

        wab.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<S3Options>>().Value;
            var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                ForcePathStyle = options.ForcePathStyle,
                AuthenticationRegion = options.Region
            };

            return new AmazonS3Client(credentials, config);
        });

        wab.Services.AddScoped<IStorageService, S3StorageService>();
    }
}
