namespace TaskManager.Application.Users.LoginUser;

public sealed record LoginUserOutput(string AccessToken, DateTimeOffset ExpiresAt);
