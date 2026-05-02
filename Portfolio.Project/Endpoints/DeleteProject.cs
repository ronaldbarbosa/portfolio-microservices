using MassTransit;
using Portfolio.Project.Domain.Interfaces;
using SharedContracts.Events;

namespace Portfolio.Project.Endpoints;

public static class DeleteProject
{
    public static async Task<IResult> Handle(long id, IProjectRepository repository, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
    {
        var project = await repository.FindByIdAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        await repository.DeleteAsync(project);
        await publishEndpoint.Publish(new ProjectDeleted(project.Id, project.Image), cancellationToken);

        return Results.NoContent();
    }
}
