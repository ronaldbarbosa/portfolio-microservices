using Portfolio.Project.Data;
using Portfolio.Project.Endpoints.Dtos;
using SharedPagination;

namespace Portfolio.Project.Endpoints;

public static class GetProjects
{
    public static async Task<IResult> Handle(int page, ProjectServiceDbContext dbContext)
    {
        const int pageSize = 5;

        var query = dbContext.Projects.Select(p => new ProjectResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Frontend,
            p.Backend,
            p.Tools,
            p.Url,
            p.Code,
            p.Image,
            p.Finished));

        var paginatedList = await PaginatedList<ProjectResponseDto>.CreateAsync(query, page, pageSize);
        return Results.Ok(paginatedList);
    }
}