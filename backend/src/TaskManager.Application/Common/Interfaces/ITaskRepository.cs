using TaskManager.Application.Common.Pagination;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<PaginatedResult<TaskEntity>> GetByUserIdPagedAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid taskId, CancellationToken cancellationToken = default);
}
