using MongoDB.Driver;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Pagination;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Models;

namespace TaskManager.Infrastructure.Persistence;

public sealed class TaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskDocument> _collection;
    private readonly TimeProvider _timeProvider;

    public TaskRepository(IMongoDatabase database, TimeProvider timeProvider)
    {
        _collection = database.GetCollection<TaskDocument>("tasks");
        _timeProvider = timeProvider;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var doc = await _collection.Find(d => d.Id == taskId).FirstOrDefaultAsync(cancellationToken);
        return doc?.ToEntity();
    }

    public async Task<PaginatedResult<TaskItem>> GetByUserIdPagedAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var offset = (page - 1) * pageSize;

        var filter = Builders<TaskDocument>.Filter.Eq(d => d.UserId, userId);
        var sort = Builders<TaskDocument>.Sort.Descending(d => d.CreatedAt);

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var docs = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip(offset)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var items = docs.Select(d => d.ToEntity()).ToList();
        return new PaginatedResult<TaskItem>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var doc = TaskDocument.FromEntity(task, _timeProvider.GetUtcNow().UtcDateTime);
        await _collection.InsertOneAsync(doc, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var update = Builders<TaskDocument>.Update
            .Set(d => d.Title, task.Title)
            .Set(d => d.Description, task.Description)
            .Set(d => d.Status, task.Status)
            .Set(d => d.DueDate, task.DueDate);

        await _collection.UpdateOneAsync(d => d.Id == task.Id, update, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid taskId, CancellationToken cancellationToken = default)
        => await _collection.DeleteOneAsync(d => d.Id == taskId, cancellationToken);
}
