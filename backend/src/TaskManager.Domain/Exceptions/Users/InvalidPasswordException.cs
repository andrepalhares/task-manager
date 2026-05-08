namespace TaskManager.Domain.Exceptions.Users;

public sealed class InvalidPasswordException()
    : DomainException(DomainErrorType.Unauthorized, "Invalid password", "The password provided is incorrect.")
{
}
