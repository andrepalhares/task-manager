using FluentValidation;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Domain.Entities;

namespace TaskManager.WebApi.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string? Status,
    DateTime? DueDate)
{
    public CreateTaskInput ToInput()
    {
        var status = DomainTaskStatus.Pending;

        if (!string.IsNullOrWhiteSpace(Status))
        {
            if (!Enum.TryParse<DomainTaskStatus>(Status, ignoreCase: false, out status))
            {
                throw new ValidationException(
                    "Status must be one of: Pending, InProgress, Completed.");
            }
        }

        return new CreateTaskInput(Title, Description, status, DueDate);
    }
}
