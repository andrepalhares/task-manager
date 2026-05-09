using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Domain.Entities;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.Infrastructure.Persistence.Models;

[ExcludeFromCodeCoverage]
internal sealed class TaskDocument
{
    [BsonId]
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DomainTaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public TaskEntity ToEntity()
        => TaskEntity.Rehydrate(Id, Title, Description, Status, DueDate, UserId);

    public static TaskDocument FromEntity(TaskEntity task, DateTime createdAt)
        => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            UserId = task.UserId,
            CreatedAt = createdAt
        };
}
