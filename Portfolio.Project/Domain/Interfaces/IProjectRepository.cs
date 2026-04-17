using Portfolio.Project.Domain.Entities;

namespace Portfolio.Project.Domain.Interfaces;

public interface IProjectRepository
{
    IQueryable<Entities.Project> GetAll();
    Task<Entities.Project?> FindByIdAsync(long id);
    Task SaveAsync(Entities.Project project);
    Task UpdateAsync(Entities.Project project);
    Task DeleteByIdAsync(long id);
}