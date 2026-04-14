using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.Endpoints.Dtos;

namespace ProjectService.Endpoints;

public static class GetProjectById
{
    public static async Task<IResult> Handle(long id, ProjectServiceDbContext dbContext)
    {
        var project = await dbContext.Projects.FindAsync(id);
        
        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

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