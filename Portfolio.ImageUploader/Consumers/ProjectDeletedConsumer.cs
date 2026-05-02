using MassTransit;
using Portfolio.ImageUploader.Services;
using SharedContracts.Events;

namespace Portfolio.ImageUploader.Consumers;

public class ProjectDeletedConsumer(IImageStorageService storageService, ILogger<ProjectDeletedConsumer> logger)
    : IConsumer<ProjectDeleted>
{
    public async Task Consume(ConsumeContext<ProjectDeleted> context)
    {
        var message = context.Message;

        if (string.IsNullOrEmpty(message.ImageUrl))
        {
            logger.LogWarning("ProjectDeleted event for project {Id} has no image URL, skipping", message.Id);
            return;
        }

        logger.LogInformation("Deleting image for project {Id}", message.Id);
        await storageService.DeleteAsync(message.ImageUrl, context.CancellationToken);
        logger.LogInformation("Image deleted for project {Id}", message.Id);
    }
}
