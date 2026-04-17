using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Domain.Entities;

namespace Portfolio.Project.Data.Repositories;

public class ProjectRepository(ProjectServiceDbContext context) : IProjectRepository
{
    private readonly ProjectServiceDbContext _context = context;
    
    public IQueryable<Domain.Entities.Project> GetAll()
    {
        return _context.Projects.AsQueryable();
    }

    public async Task<Domain.Entities.Project?> FindByIdAsync(long id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task SaveAsync(Domain.Entities.Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}