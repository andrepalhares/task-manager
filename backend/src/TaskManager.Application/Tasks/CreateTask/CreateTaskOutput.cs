namespace TaskManager.Application.Tasks.CreateTask;

public sealed record CreateTaskOutput(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    Guid UserId);
