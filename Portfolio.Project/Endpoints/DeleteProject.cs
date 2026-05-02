using MassTransit;
using Portfolio.Project.Domain.Interfaces;
using SharedContracts.Events;

namespace Portfolio.Project.Endpoints;

public static class DeleteProject
{
    public static async Task<IResult> Handle(long id, IProjectRepository repository, IPublishEndpoint publishEndpoint, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        var project = await repository.FindByIdAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        await repository.DeleteAsync(project);

        var logger = loggerFactory.CreateLogger("DeleteProject");
        try
        {
            await publishEndpoint.Publish(new ProjectDeleted(project.Id, project.Image), cancellationToken);
            logger.LogInformation("Published ProjectDeleted event for project {Id}", project.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish ProjectDeleted event for project {Id}", project.Id);
        }

        return Results.NoContent();
    }
}
