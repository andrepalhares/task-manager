using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Users.RegisterUser;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Users;

namespace TaskManager.Application.Tests.Users.RegisterUser;

public class RegisterUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IValidator<RegisterUserInput> _validator = new RegisterUserInputValidator();
    private readonly RegisterUserUseCase _useCase;

    public RegisterUserUseCaseTests()
    {
        _passwordHasher.Hash(Arg.Any<string>()).Returns(call => $"hashed::{call.Arg<string>()}");
        _useCase = new RegisterUserUseCase(_userRepository, _passwordHasher, _validator);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_HashesPasswordAndPersistsUser()
    {
        var input = new RegisterUserInput("user@example.com", "Password1!", "Alice");

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Email.ShouldBe("user@example.com");
        result.Name.ShouldBe("Alice");

        _passwordHasher.Received(1).Hash("Password1!");
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u =>
                u.Email == "user@example.com"
                && u.Name == "Alice"
                && u.PasswordHash == "hashed::Password1!"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_NormalizesEmailToLowercase()
    {
        var input = new RegisterUserInput("User@Example.COM", "Password1!", "Alice");

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        result.Email.ShouldBe("user@example.com");
    }

    [Fact]
    public async Task ExecuteAsync_TrimsWhitespaceFromName()
    {
        var input = new RegisterUserInput("user@example.com", "Password1!", "  Alice  ");

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        result.Name.ShouldBe("Alice");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidInput_ThrowsValidationExceptionAndDoesNotPersist()
    {
        var input = new RegisterUserInput("", "", "");

        await Should.ThrowAsync<ValidationException>(
            () => _useCase.ExecuteAsync(input, CancellationToken.None));

        _passwordHasher.DidNotReceiveWithAnyArgs().Hash(default!);
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsEmailAlreadyTaken_PropagatesException()
    {
        _userRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new EmailAlreadyTakenException("user@example.com"));

        await Should.ThrowAsync<EmailAlreadyTakenException>(
            () => _useCase.ExecuteAsync(
                new RegisterUserInput("user@example.com", "Password1!", "Alice"),
                CancellationToken.None));
    }
}
