using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.Application.Tasks.CreateTask;

public sealed record CreateTaskInput(
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTime? DueDate);
