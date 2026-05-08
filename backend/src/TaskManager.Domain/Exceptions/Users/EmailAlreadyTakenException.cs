namespace TaskManager.Domain.Exceptions.Users;

public sealed class EmailAlreadyTakenException(string email)
    : DomainException(DomainErrorType.Conflict, "Email already registered", $"Email '{email}' is already registered.")
{
    public string Email { get; } = email;
}
