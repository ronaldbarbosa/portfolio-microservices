using Microsoft.EntityFrameworkCore;
using Portfolio.Project.Data;
using Portfolio.Project.Endpoints.Dtos;

namespace Portfolio.Project.Endpoints;

public static class UpdateProject
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

    public static async Task<IResult> Handle(long id, Request request, ProjectServiceDbContext dbContext)
    {
        var project = await dbContext.Projects.FindAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        project.Name = request.Name;
        project.Description = request.Description;
        project.Frontend = request.Frontend;
        project.Backend = request.Backend;
        project.Tools = request.Tools;
        project.Url = request.Url;
        project.Code = request.Code;
        project.Image = request.Image;
        project.Finished = request.Finished;

        await dbContext.SaveChangesAsync();

        var response = new ProjectResponseDto(
            project.Id,
            project.Name,
            project.Description,
            project.Frontend,
            project.Backend,
            project.Tools,
            project.Url,
            project.Code,
            project.Image,
            project.Finished);

        return Results.Ok(response);
    }
}