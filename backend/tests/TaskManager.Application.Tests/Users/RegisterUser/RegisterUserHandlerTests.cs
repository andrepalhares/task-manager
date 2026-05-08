using FluentValidation;
using NSubstitute;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Users.RegisterUser;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Tests.Users.RegisterUser;

public class RegisterUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IValidator<RegisterUserInput> _validator = new RegisterUserInputValidator();
    private readonly RegisterUserUseCase _useCase;

    public RegisterUserUseCaseTests()
    {
        _useCase = new RegisterUserUseCase(_userRepository, _passwordHasher, _validator);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_HashesPasswordAndPersistsUser()
    {
        var input = new RegisterUserInput("jane@example.com", "password123", "Jane Doe");
        var hashedPassword = "hashed_password";
        _passwordHasher.Hash(input.Password).Returns(hashedPassword);

        var result = await _useCase.ExecuteAsync(input);

        _passwordHasher.Received(1).Hash(input.Password);
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u => u.PasswordHash == hashedPassword), default);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsRegisterUserOutputWithGeneratedId()
    {
        var input = new RegisterUserInput("jane@example.com", "password123", "Jane Doe");
        var hashedPassword = "hashed_password";
        _passwordHasher.Hash(input.Password).Returns(hashedPassword);

        var result = await _useCase.ExecuteAsync(input);

        result.Email.ShouldBe(input.Email);
        result.Name.ShouldBe(input.Name);
        result.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task ExecuteAsync_TrimsWhitespaceFromName()
    {
        var input = new RegisterUserInput("jane@example.com", "hunter2!!", "  Jane  ");
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed");

        await _useCase.ExecuteAsync(input);

        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Name == "Jane"),
            Arg.Any<CancellationToken>());
    }
}
