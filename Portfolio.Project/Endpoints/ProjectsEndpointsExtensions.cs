using Portfolio.Project.Endpoints.Dtos;
using SharedPagination;

namespace Portfolio.Project.Endpoints;

public static class ProjectsEndpointsExtensions
{
    public static WebApplication MapProjectsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects");
        
        group.MapGet("", GetProjects.Handle)
            .WithName("ListProjects")
            .Produces<PaginatedList<ProjectResponseDto>>();
        
        group.MapGet("/{id:long}", GetProjectById.Handle)
            .WithName("GetProject")
            .Produces<ProjectResponseDto>()
            .Produces(404);

        group.MapPost("", CreateProject.Handle)
            .WithName("CreateProject")
            .Produces<ProjectResponseDto>(201);

        group.MapPut("/{id:long}", UpdateProject.Handle)
            .WithName("UpdateProject")
            .Produces<ProjectResponseDto>()
            .Produces(404);

        group.MapDelete("/{id:long}", DeleteProject.Handle)
            .WithName("DeleteProject")
            .Produces(204)
            .Produces(404);
        
        return app;
    }
}