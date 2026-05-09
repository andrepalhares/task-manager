using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(UserEntity user, CancellationToken cancellationToken = default);
}
