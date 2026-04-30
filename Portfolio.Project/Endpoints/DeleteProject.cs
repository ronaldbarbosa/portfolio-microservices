using Portfolio.Project.Domain.Interfaces;

namespace Portfolio.Project.Endpoints;

public static class DeleteProject
{
    public static async Task<IResult> Handle(long id, IProjectRepository repository)
    {
        var project = await repository.FindByIdAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        await repository.DeleteAsync(project);

        return Results.NoContent();
    }
}