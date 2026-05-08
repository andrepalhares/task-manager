using TaskManager.Application.Users.RegisterUser;

namespace TaskManager.WebApi.Auth;

public sealed record RegisterUserRequest(string Email, string Password, string Name)
{
    public RegisterUserInput ToInput() => new(Email, Password, Name);
}
