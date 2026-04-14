using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using SharedPagination;

namespace ProjectService.Endpoints;

public static class GetProjects
{
    public record ProjectListResponse(
        long Id,
        string Name,
        string Description);

    public static async Task<IResult> Handle(int page, ProjectServiceDbContext dbContext)
    {
        const int pageSize = 5;

        var projects = await dbContext.Projects.ToListAsync();
        var projectsDtos = projects.Select(p => new ProjectListResponse(
            p.Id,
            p.Name,
            p.Description))
            .ToList();

        var totalCount = projectsDtos.Count;
        var items = projectsDtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var paginatedList = new PaginatedList<ProjectListResponse>(items, totalCount, page, pageSize);
        return Results.Ok(paginatedList);
    }
}