namespace TaskManager.Application.Tasks.UpdateTask;

public sealed record UpdateTaskOutput(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    Guid UserId);
