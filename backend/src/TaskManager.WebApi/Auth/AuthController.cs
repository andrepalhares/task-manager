using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Users.LoginUser;
using TaskManager.Application.Users.RegisterUser;
using TaskManager.WebApi.Common;

namespace TaskManager.WebApi.Auth;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IUseCase<RegisterUserInput, RegisterUserOutput> _registerUserUseCase;
    private readonly IUseCase<LoginUserInput, LoginUserOutput> _loginUserUseCase;

    public AuthController(
        IUseCase<RegisterUserInput, RegisterUserOutput> registerUserUseCase,
        IUseCase<LoginUserInput, LoginUserOutput> loginUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUserUseCase = loginUserUseCase;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await _registerUserUseCase.ExecuteAsync(request.ToInput(), cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await _loginUserUseCase.ExecuteAsync(request.ToInput(), cancellationToken);
        return Ok(result);
    }
}
