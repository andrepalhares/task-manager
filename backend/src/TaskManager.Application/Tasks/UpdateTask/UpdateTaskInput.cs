using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.Application.Tasks.UpdateTask;

public sealed record UpdateTaskInput(
    Guid TaskId,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTime? DueDate);
