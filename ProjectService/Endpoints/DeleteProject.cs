using Microsoft.EntityFrameworkCore;
using ProjectService.Data;

namespace ProjectService.Endpoints;

public static class DeleteProject
{
    public static async Task<IResult> Handle(long id, ProjectServiceDbContext dbContext)
    {
        var project = await dbContext.Projects.FindAsync(id);

        if (project is null)
            return Results.NotFound(new { message = "Project not found" });

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}