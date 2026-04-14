using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.Endpoints.Dtos;
using SharedPagination;

namespace ProjectService.Endpoints;

public static class GetProjects
{
    public static async Task<IResult> Handle(int page, ProjectServiceDbContext dbContext)
    {
        const int pageSize = 5;

        var projects = await dbContext.Projects.ToListAsync();
        var projectsDtos = projects.Select(p => new ProjectResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Frontend,
            p.Backend,
            p.Tools,
            p.Url,
            p.Code,
            p.Image,
            p.Finished))
            .ToList();

        var totalCount = projectsDtos.Count;
        var items = projectsDtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var paginatedList = new PaginatedList<ProjectResponseDto>(items, totalCount, page, pageSize);
        return Results.Ok(paginatedList);
    }
}