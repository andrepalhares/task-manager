namespace TaskManager.Domain.Exceptions.Tasks;

public sealed class TaskNotFoundException : DomainException
{
    public TaskNotFoundException(Guid taskId)
        : base(DomainErrorType.NotFound, "Task not found", $"Task '{taskId}' was not found.")
    {
        TaskId = taskId;
    }

    public Guid TaskId { get; }
}
