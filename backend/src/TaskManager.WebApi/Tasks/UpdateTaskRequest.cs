using FluentValidation;
using TaskManager.Application.Tasks.UpdateTask;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.WebApi.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    string? Status,
    DateTime? DueDate)
{
    public UpdateTaskInput ToInput(Guid taskId)
    {
        if (string.IsNullOrWhiteSpace(Status) ||
            !Enum.TryParse<DomainTaskStatus>(Status, ignoreCase: false, out var parsed))
        {
            throw new ValidationException(
                "Status must be one of: Pending, InProgress, Completed.");
        }

        return new UpdateTaskInput(taskId, Title, Description, parsed, DueDate);
    }
}
