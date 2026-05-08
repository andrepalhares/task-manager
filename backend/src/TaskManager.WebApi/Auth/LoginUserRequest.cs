using TaskManager.Application.Users.LoginUser;

namespace TaskManager.WebApi.Auth;

public sealed record LoginUserRequest(string Email, string Password)
{
    public LoginUserInput ToInput() => new(Email, Password);
}
