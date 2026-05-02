using Portfolio.ImageUploader.Endpoints.Dtos;

namespace Portfolio.ImageUploader.Endpoints;

public static class ImageEndpointsExtensions
{
    public static WebApplication MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/images");

        group.MapPost("", UploadImage.Handle)
            .WithName("UploadImage")
            .Produces<ImageUploadResponseDto>()
            .DisableAntiforgery();

        return app;
    }
}
