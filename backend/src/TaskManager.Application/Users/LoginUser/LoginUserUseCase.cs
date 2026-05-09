using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Users;

namespace TaskManager.Application.Users.LoginUser;

public sealed class LoginUserUseCase : IUseCase<LoginUserInput, LoginUserOutput>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IValidator<LoginUserInput> _validator;

    public LoginUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenIssuer tokenIssuer,
        IValidator<LoginUserInput> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
        _validator = validator;
    }

    public async Task<LoginUserOutput> ExecuteAsync(
        LoginUserInput input,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var user = await GetUserByEmailAsync(input.Email, cancellationToken);

        VerifyPassword(input.Password, user.PasswordHash);

        var token = _tokenIssuer.CreateToken(user);

        return new LoginUserOutput(token.Value, token.ExpiresAt);
    }

    private async Task<UserEntity> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException();
        }
        return user;
    }

    private void VerifyPassword(string password, string hash)
    {
        if (!_passwordHasher.Verify(password, hash))
        {
            throw new InvalidPasswordException();
        }
    }
}
