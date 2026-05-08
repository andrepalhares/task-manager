namespace TaskManager.Domain.Exceptions.Users;

public sealed class EmailAlreadyTakenException(string email)
    : Exception($"Email '{email}' is already registered.")
{
    public string Email { get; } = email;
}
