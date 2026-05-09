using MongoDB.Driver;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Users;

namespace TaskManager.Infrastructure.Persistence.Users;

public sealed class UserRepository : IUserRepository
{
    // MongoDB error code 11000 = E11000 duplicate key error.
    // See: https://www.mongodb.com/docs/manual/reference/error-codes/
    private const int DuplicateKeyErrorCode = 11000;

    private readonly IMongoCollection<UserEntity> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserEntity>("users");
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _collection.Find(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Code == DuplicateKeyErrorCode)
        {
            throw new EmailAlreadyTakenException(user.Email);
        }
    }
}
