namespace TaskManager.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(DomainErrorType errorType, string title, string message)
        : base(message)
    {
        ErrorType = errorType;
        Title = title;
    }

    public DomainErrorType ErrorType { get; }

    public string Title { get; }
}
