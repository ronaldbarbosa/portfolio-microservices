using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Endpoints.Dtos;

namespace Portfolio.Project.Endpoints;

public static class GetProjectById
{
    public static async Task<IResult> Handle(long id, IProjectRepository repository)
    {
        var project = await repository.FindByIdAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        return Results.Ok(new ProjectResponseDto(
            project.Id, project.Name, project.Description, project.Frontend, project.Backend,
            project.Tools, project.Url, project.Code, project.Image, project.Finished));
    }
}