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
        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }

    private static string ExtractPublicId(string imageUrl)
    {
        var path = new Uri(imageUrl).AbsolutePath;
        var segments = path.Split('/');
        var uploadIndex = Array.IndexOf(segments, "upload");
        var publicIdWithExtension = string.Join("/", segments.Skip(uploadIndex + 2));
        return Path.GetFileNameWithoutExtension(publicIdWithExtension);
    }
}
