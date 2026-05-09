using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Domain.Entities;

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

    public TaskItem ToEntity()
        => TaskItem.Rehydrate(Id, Title, Description, Status, DueDate, UserId);

    public static TaskDocument FromEntity(TaskItem task, DateTime createdAt)
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
