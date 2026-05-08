namespace TaskManager.Application.Tasks.GetTaskById;

public sealed record GetTaskByIdOutput(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    Guid UserId);
