using ProjectService.Domain.Entities;
using ProjectService.Domain.Interfaces;

namespace ProjectService.Data.Repositories;

public class ProjectRepository(ProjectServiceDbContext context) : IProjectRepository
{
    private readonly ProjectServiceDbContext _context = context;
    
    public IQueryable<Project> GetAll()
    {
        return _context.Projects.AsQueryable();
    }

    public async Task<Project?> FindByIdAsync(long id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task SaveAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
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