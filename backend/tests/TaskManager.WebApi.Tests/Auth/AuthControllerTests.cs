using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Users.LoginUser;
using TaskManager.Application.Users.RegisterUser;
using TaskManager.WebApi.Auth;

namespace TaskManager.WebApi.Tests.Auth;

public class AuthControllerTests
{
    private readonly IUseCase<RegisterUserInput, RegisterUserOutput> _registerUseCase
        = Substitute.For<IUseCase<RegisterUserInput, RegisterUserOutput>>();
    private readonly IUseCase<LoginUserInput, LoginUserOutput> _loginUseCase
        = Substitute.For<IUseCase<LoginUserInput, LoginUserOutput>>();

    private AuthController CreateSut()
    {
        var controller = new AuthController(_registerUseCase, _loginUseCase);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public async Task Register_DelegatesToUseCaseAndReturnsCreatedAtAction()
    {
        var request = new RegisterUserRequest("user@example.com", "Password1!", "Alice");
        var output = new RegisterUserOutput(Guid.NewGuid(), "user@example.com", "Alice");
        _registerUseCase
            .ExecuteAsync(Arg.Any<RegisterUserInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var result = await CreateSut().Register(request, CancellationToken.None);

        var created = result.ShouldBeOfType<CreatedAtActionResult>();
        created.Value.ShouldBe(output);
        created.RouteValues!["id"].ShouldBe(output.Id);

        await _registerUseCase.Received(1).ExecuteAsync(
            Arg.Is<RegisterUserInput>(i =>
                i.Email == "user@example.com"
                && i.Password == "Password1!"
                && i.Name == "Alice"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_DelegatesToUseCaseAndReturnsOkWithToken()
    {
        var request = new LoginUserRequest("user@example.com", "Password1!");
        var output = new LoginUserOutput("jwt-token", DateTimeOffset.UtcNow.AddHours(1));
        _loginUseCase
            .ExecuteAsync(Arg.Any<LoginUserInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var result = await CreateSut().Login(request, CancellationToken.None);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(output);

        await _loginUseCase.Received(1).ExecuteAsync(
            Arg.Is<LoginUserInput>(i => i.Email == "user@example.com" && i.Password == "Password1!"),
            Arg.Any<CancellationToken>());
    }
}
