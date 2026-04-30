using Portfolio.Project.Domain.Interfaces;
using SharedPagination;

namespace Portfolio.Project.Data.Repositories;

public class ProjectRepository(ProjectServiceDbContext context) : IProjectRepository
{
    public async Task<PaginatedList<Domain.Entities.Project>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await PaginatedList<Domain.Entities.Project>.CreateAsync(
            context.Projects.AsQueryable(), pageNumber, pageSize);
    }

    public async Task<Domain.Entities.Project?> FindByIdAsync(long id)
    {
        return await context.Projects.FindAsync(id);
    }

    public async Task SaveAsync(Domain.Entities.Project project)
    {
        context.Projects.Add(project);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Project project)
    {
        context.Projects.Update(project);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Domain.Entities.Project project)
    {
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}