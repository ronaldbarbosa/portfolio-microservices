using ProjectService.Domain.Entities;

namespace ProjectService.Domain.Interfaces;

public interface IProjectRepository
{
    IQueryable<Project> GetAll();
    Task<Project?> FindByIdAsync(long id);
    Task SaveAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteByIdAsync(long id);
}