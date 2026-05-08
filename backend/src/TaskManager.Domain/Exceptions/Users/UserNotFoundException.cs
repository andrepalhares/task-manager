namespace TaskManager.Domain.Exceptions.Users;

public sealed class UserNotFoundException()
    : Exception("No user found with the provided email.")
{
}
