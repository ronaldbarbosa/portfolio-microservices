using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Portfolio.ImageUploader.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(IConfiguration config)
    {
        var cloudName  = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME") ?? config["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException("CLOUDINARY_CLOUD_NAME is not configured");
        var apiKey     = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")    ?? config["Cloudinary:ApiKey"]
            ?? throw new InvalidOperationException("CLOUDINARY_API_KEY is not configured");
        var apiSecret  = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? config["Cloudinary:ApiSecret"]
            ?? throw new InvalidOperationException("CLOUDINARY_API_SECRET is not configured");

        _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
    }

    public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream)
        };
        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result.SecureUrl.ToString();
    }

    public async Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var publicId = ExtractPublicId(imageUrl);
        var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        if (result.Result != "ok")
            throw new InvalidOperationException($"Cloudinary returned '{result.Result}' when deleting '{publicId}'");
    }

    private static string ExtractPublicId(string imageUrl)
    {
        var segments = new Uri(imageUrl).AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        var uploadIndex = Array.IndexOf(segments, "upload");
        if (uploadIndex < 0)
            throw new ArgumentException($"Not a valid Cloudinary URL: {imageUrl}");

        var afterUpload = segments.Skip(uploadIndex + 1).ToArray();
        // skip version segment (e.g. "v1312461204")
        var start = afterUpload.Length > 0
            && afterUpload[0].Length > 1
            && afterUpload[0][0] == 'v'
            && afterUpload[0][1..].All(char.IsDigit) ? 1 : 0;

        var publicIdWithExtension = string.Join("/", afterUpload.Skip(start));
        var lastDot = publicIdWithExtension.LastIndexOf('.');
        return lastDot >= 0 ? publicIdWithExtension[..lastDot] : publicIdWithExtension;
    }
}
