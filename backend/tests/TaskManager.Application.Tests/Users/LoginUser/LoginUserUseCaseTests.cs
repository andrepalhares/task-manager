using FluentValidation;
using NSubstitute;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Users.LoginUser;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Users;

namespace TaskManager.Application.Tests.Users.LoginUser;

public class LoginUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenIssuer _tokenService = Substitute.For<ITokenIssuer>();
    private readonly IValidator<LoginUserInput> _validator = new LoginUserInputValidator();
    private readonly LoginUserUseCase _useCase;

    public LoginUserUseCaseTests()
    {
        _useCase = new LoginUserUseCase(_userRepository, _passwordHasher, _tokenService, _validator);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsAccessTokenAndExpiresAt()
    {
        var input = new LoginUserInput("jane@example.com", "password123");
        var userId = Guid.NewGuid();
        var user = UserEntity.Create(input.Email, "hashed_password", "Jane Doe");
        
        _userRepository.GetByEmailAsync(input.Email, default).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(true);
        
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(60);
        var token = new AuthToken("jwt_token_value", expiresAt);
        _tokenService.CreateToken(user).Returns(token);

        var result = await _useCase.ExecuteAsync(input);

        result.AccessToken.ShouldBe("jwt_token_value");
        result.ExpiresAt.ShouldBe(expiresAt);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_DoesNotIncludeUserFieldsInOutput()
    {
        var input = new LoginUserInput("jane@example.com", "password123");
        var user = UserEntity.Create(input.Email, "hashed_password", "Jane Doe");

        _userRepository.GetByEmailAsync(input.Email, default).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(true);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(60);
        var token = new AuthToken("jwt_token_value", expiresAt);
        _tokenService.CreateToken(user).Returns(token);

        var result = await _useCase.ExecuteAsync(input);

        result.AccessToken.ShouldBe("jwt_token_value");
        result.ExpiresAt.ShouldBe(expiresAt);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownEmail_ThrowsUserNotFoundException()
    {
        var input = new LoginUserInput("unknown@example.com", "password123");
        _userRepository.GetByEmailAsync(input.Email, default).Returns((UserEntity?)null);

        await Should.ThrowAsync<UserNotFoundException>(
            () => _useCase.ExecuteAsync(input));
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_ThrowsInvalidPasswordException()
    {
        var input = new LoginUserInput("jane@example.com", "wrongpassword");
        var user = UserEntity.Create(input.Email, "hashed_password", "Jane Doe");
        
        _userRepository.GetByEmailAsync(input.Email, default).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(false);

        await Should.ThrowAsync<InvalidPasswordException>(
            () => _useCase.ExecuteAsync(input));
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_DoesNotCallTokenService()
    {
        var input = new LoginUserInput("jane@example.com", "wrongpassword");
        var user = UserEntity.Create(input.Email, "hashed_password", "Jane Doe");
        
        _userRepository.GetByEmailAsync(input.Email, default).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(false);

        try
        {
            await _useCase.ExecuteAsync(input);
        }
        catch (InvalidPasswordException)
        {
        }

        _tokenService.DidNotReceive().CreateToken(Arg.Any<UserEntity>());
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyEmail_ThrowsValidationException_AndDoesNotHitRepository()
    {
        var input = new LoginUserInput("", "password123");

        await Should.ThrowAsync<ValidationException>(
            () => _useCase.ExecuteAsync(input));

        await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>(), default);
    }
}
