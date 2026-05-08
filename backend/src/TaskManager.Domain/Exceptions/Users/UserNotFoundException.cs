namespace TaskManager.Domain.Exceptions.Users;

public sealed class UserNotFoundException()
    : DomainException(DomainErrorType.Unauthorized, "User not found", "No user found with the provided email.")
{
}
