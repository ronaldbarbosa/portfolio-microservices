using SharedPagination;

namespace ProjectService.Endpoints;

public static class ProjectsEndpointsExtensions
{
    public static WebApplication MapProjectsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects");
        
        group.MapGet("", GetProjects.Handle)
            .WithName("ListProjects")
            .Produces<PaginatedList<GetProjects.ProjectListResponse>>();
        
        return app;
    }
}