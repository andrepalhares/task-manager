using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Exceptions.Users;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.WebApi.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var response = httpContext.Response;
        response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = StatusCodes.Status400BadRequest;
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };

                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                problemDetails.Extensions["errors"] = errors;
                await response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;

            case EmailAlreadyTakenException emailException:
                response.StatusCode = StatusCodes.Status409Conflict;
                var conflictProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Email Already Registered",
                    Detail = emailException.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
                };
                await response.WriteAsJsonAsync(conflictProblemDetails, cancellationToken);
                return true;

            case UserNotFoundException userException:
                response.StatusCode = StatusCodes.Status401Unauthorized;
                var userNotFoundProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "User not found",
                    Detail = userException.Message,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                };
                await response.WriteAsJsonAsync(userNotFoundProblemDetails, cancellationToken);
                return true;

            case InvalidPasswordException passwordException:
                response.StatusCode = StatusCodes.Status401Unauthorized;
                var invalidPasswordProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid password",
                    Detail = passwordException.Message,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                };
                await response.WriteAsJsonAsync(invalidPasswordProblemDetails, cancellationToken);
                return true;

            case TaskNotFoundException tnf:
                response.StatusCode = StatusCodes.Status404NotFound;
                await response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Task not found",
                    Detail = tnf.Message
                }, cancellationToken);
                return true;

            case TaskAccessForbiddenException taf:
                response.StatusCode = StatusCodes.Status403Forbidden;
                await response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You do not have access to this task."
                }, cancellationToken);
                return true;

            default:
                return false;
        }
    }
}
