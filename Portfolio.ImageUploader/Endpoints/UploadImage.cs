using Portfolio.ImageUploader.Endpoints.Dtos;
using Portfolio.ImageUploader.Services;

namespace Portfolio.ImageUploader.Endpoints;

public static class UploadImage
{
    public static async Task<IResult> Handle(IFormFile file, IImageStorageService storageService, CancellationToken cancellationToken)
    {
        var url = await storageService.UploadAsync(file, cancellationToken);
        return Results.Ok(new ImageUploadResponseDto(url));
    }
}
