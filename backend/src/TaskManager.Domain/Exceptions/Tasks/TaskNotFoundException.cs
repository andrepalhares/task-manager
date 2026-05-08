namespace TaskManager.Domain.Exceptions.Tasks;

public sealed class TaskNotFoundException : Exception
{
    public TaskNotFoundException(Guid taskId)
        : base($"Task '{taskId}' was not found.")
    {
        TaskId = taskId;
    }

    public Guid TaskId { get; }
}
