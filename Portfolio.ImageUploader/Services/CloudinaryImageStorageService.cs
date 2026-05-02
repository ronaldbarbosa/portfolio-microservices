using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Portfolio.ImageUploader.Services;

public class CloudinaryImageStorageService(IConfiguration config) : IImageStorageService
{
    private readonly Cloudinary _cloudinary = new(new Account(
        config["Cloudinary:CloudName"],
        config["Cloudinary:ApiKey"],
        config["Cloudinary:ApiSecret"]
    ));

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
