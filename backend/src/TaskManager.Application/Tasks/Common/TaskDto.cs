using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tasks.Common;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    Guid UserId)
{
    public static TaskDto FromEntity(TaskEntity task)
        => new(task.Id, task.Title, task.Description, task.Status.ToString(), task.DueDate, task.UserId);
}
