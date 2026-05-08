using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Shouldly;
using TaskManager.Domain.Exceptions.Tasks;
using TaskManager.Domain.Exceptions.Users;
using TaskManager.WebApi.Middleware;

namespace TaskManager.WebApi.Tests.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _sut = new();

    private static (DefaultHttpContext ctx, MemoryStream body) NewContext()
    {
        var ctx = new DefaultHttpContext();
        var body = new MemoryStream();
        ctx.Response.Body = body;
        return (ctx, body);
    }

    private static async Task<JsonElement> ReadBodyAsync(MemoryStream body)
    {
        body.Position = 0;
        using var doc = await JsonDocument.ParseAsync(body);
        return doc.RootElement.Clone();
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_ReturnsBadRequestWithGroupedErrors()
    {
        var (ctx, body) = NewContext();
        var failures = new[]
        {
            new ValidationFailure("Title", "Title is required."),
            new ValidationFailure("Title", "Title is too short."),
            new ValidationFailure("DueDate", "DueDate must be in the future.")
        };
        var ex = new ValidationException(failures);

        var handled = await _sut.TryHandleAsync(ctx, ex, CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        ctx.Response.ContentType.ShouldStartWith("application/json");

        var root = await ReadBodyAsync(body);
        root.GetProperty("status").GetInt32().ShouldBe(400);
        root.GetProperty("title").GetString().ShouldBe("Validation Failed");
        var errors = root.GetProperty("errors");
        errors.GetProperty("Title").GetArrayLength().ShouldBe(2);
        errors.GetProperty("DueDate").GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task TryHandleAsync_EmailAlreadyTakenException_ReturnsConflict()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(
            ctx, new EmailAlreadyTakenException("user@example.com"), CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
        var root = await ReadBodyAsync(body);
        root.GetProperty("status").GetInt32().ShouldBe(409);
    }

    [Fact]
    public async Task TryHandleAsync_UserNotFoundException_ReturnsUnauthorized()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(ctx, new UserNotFoundException(), CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        (await ReadBodyAsync(body)).GetProperty("status").GetInt32().ShouldBe(401);
    }

    [Fact]
    public async Task TryHandleAsync_InvalidPasswordException_ReturnsUnauthorized()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(ctx, new InvalidPasswordException(), CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        (await ReadBodyAsync(body)).GetProperty("status").GetInt32().ShouldBe(401);
    }

    [Fact]
    public async Task TryHandleAsync_TaskNotFoundException_ReturnsNotFound()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(
            ctx, new TaskNotFoundException(Guid.NewGuid()), CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        (await ReadBodyAsync(body)).GetProperty("status").GetInt32().ShouldBe(404);
    }

    [Fact]
    public async Task TryHandleAsync_TaskAccessForbiddenException_ReturnsForbidden()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(
            ctx, new TaskAccessForbiddenException(Guid.NewGuid()), CancellationToken.None);

        handled.ShouldBeTrue();
        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        (await ReadBodyAsync(body)).GetProperty("status").GetInt32().ShouldBe(403);
    }

    [Fact]
    public async Task TryHandleAsync_UnknownException_ReturnsFalseWithoutWritingBody()
    {
        var (ctx, body) = NewContext();
        var handled = await _sut.TryHandleAsync(ctx, new InvalidOperationException("boom"), CancellationToken.None);

        handled.ShouldBeFalse();
        body.Length.ShouldBe(0);
    }
}
