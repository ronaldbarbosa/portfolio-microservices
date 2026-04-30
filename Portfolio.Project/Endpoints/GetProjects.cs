using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Endpoints.Dtos;
using SharedPagination;

namespace Portfolio.Project.Endpoints;

public static class GetProjects
{
    public static async Task<IResult> Handle(int page, IProjectRepository repository)
    {
        const int pageSize = 5;

        var projects = await repository.GetAllAsync(page, pageSize);
        var dtoItems = projects.Items
            .Select(p => new ProjectResponseDto(
                p.Id, p.Name, p.Description, p.Frontend, p.Backend,
                p.Tools, p.Url, p.Code, p.Image, p.Finished))
            .ToList();

        return Results.Ok(new PaginatedList<ProjectResponseDto>(
            dtoItems, projects.TotalItemCount, projects.PageNumber, projects.PageSize));
    }
}