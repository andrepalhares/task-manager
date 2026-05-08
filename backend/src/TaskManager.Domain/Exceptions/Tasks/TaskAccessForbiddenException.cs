namespace TaskManager.Domain.Exceptions.Tasks;

public sealed class TaskAccessForbiddenException : Exception
{
    public TaskAccessForbiddenException(Guid taskId)
        : base($"Access to task '{taskId}' is forbidden.")
    {
        TaskId = taskId;
    }

    public Guid TaskId { get; }
}
