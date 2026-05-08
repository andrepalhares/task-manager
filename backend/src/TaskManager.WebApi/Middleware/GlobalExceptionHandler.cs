using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Exceptions;

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

            case DomainException domainException:
                var statusCode = MapStatusCode(domainException.ErrorType);
                response.StatusCode = statusCode;
                await response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = statusCode,
                    Title = domainException.Title,
                    Detail = domainException.Message,
                    Type = MapType(domainException.ErrorType)
                }, cancellationToken);
                return true;

            default:
                return false;
        }
    }

    private static int MapStatusCode(DomainErrorType errorType) => errorType switch
    {
        DomainErrorType.NotFound => StatusCodes.Status404NotFound,
        DomainErrorType.Conflict => StatusCodes.Status409Conflict,
        DomainErrorType.Forbidden => StatusCodes.Status403Forbidden,
        DomainErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string MapType(DomainErrorType errorType) => errorType switch
    {
        DomainErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        DomainErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        DomainErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        DomainErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        _ => "about:blank"
    };
}
