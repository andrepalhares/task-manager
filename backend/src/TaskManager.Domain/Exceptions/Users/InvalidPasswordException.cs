namespace TaskManager.Domain.Exceptions.Users;

public sealed class InvalidPasswordException()
    : Exception("The password provided is incorrect.")
{
}
