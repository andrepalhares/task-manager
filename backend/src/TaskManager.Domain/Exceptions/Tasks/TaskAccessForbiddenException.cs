namespace TaskManager.Domain.Exceptions.Tasks;

public sealed class TaskAccessForbiddenException : DomainException
{
    public TaskAccessForbiddenException(Guid taskId)
        : base(DomainErrorType.Forbidden, "Forbidden", $"Access to task '{taskId}' is forbidden.")
    {
        TaskId = taskId;
    }

    public Guid TaskId { get; }
}
