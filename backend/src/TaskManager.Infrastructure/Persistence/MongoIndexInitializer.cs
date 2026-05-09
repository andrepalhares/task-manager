using MongoDB.Driver;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Models;

namespace TaskManager.Infrastructure.Persistence;

public static class MongoIndexInitializer
{
    public static async Task EnsureIndexesAsync(IMongoDatabase database, CancellationToken cancellationToken = default)
    {
        var users = database.GetCollection<User>("users");
        await users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_users_email" }),
            cancellationToken: cancellationToken);

        var tasks = database.GetCollection<TaskDocument>("tasks");
        await tasks.Indexes.CreateOneAsync(
            new CreateIndexModel<TaskDocument>(
                Builders<TaskDocument>.IndexKeys
                    .Ascending(t => t.UserId)
                    .Descending(t => t.CreatedAt),
                new CreateIndexOptions { Name = "ix_tasks_userId_createdAt" }),
            cancellationToken: cancellationToken);
    }
}
