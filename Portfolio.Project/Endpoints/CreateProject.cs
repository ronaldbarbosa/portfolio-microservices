using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Endpoints.Dtos;

namespace Portfolio.Project.Endpoints;

public static class CreateProject
{
    public record Request(
        string Name,
        string Description,
        string Frontend,
        string Backend,
        string Tools,
        string Url,
        string Code,
        string Image,
        bool Finished);

    public static async Task<IResult> Handle(Request request, IProjectRepository repository)
    {
        var project = new Domain.Entities.Project
        {
            Name = request.Name,
            Description = request.Description,
            Frontend = request.Frontend,
            Backend = request.Backend,
            Tools = request.Tools,
            Url = request.Url,
            Code = request.Code,
            Image = request.Image,
            Finished = request.Finished
        };

        await repository.SaveAsync(project);

        return Results.Created($"/api/projects/{project.Id}", new ProjectResponseDto(
            project.Id, project.Name, project.Description, project.Frontend, project.Backend,
            project.Tools, project.Url, project.Code, project.Image, project.Finished));
    }
}