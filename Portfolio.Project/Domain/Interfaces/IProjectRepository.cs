using SharedPagination;

namespace Portfolio.Project.Domain.Interfaces;

public interface IProjectRepository
{
    Task<PaginatedList<Entities.Project>> GetAllAsync(int pageNumber, int pageSize);
    Task<Entities.Project?> FindByIdAsync(long id);
    Task SaveAsync(Entities.Project project);
    Task UpdateAsync(Entities.Project project);
    Task DeleteAsync(Entities.Project project);
}