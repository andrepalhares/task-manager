using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace TaskManager.WebApi.Common;

/// <summary>
/// Base controller providing automatic access to request cancellation token
/// and other common controller utilities
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the cancellation token for the current HTTP request.
    /// This token is triggered when the client disconnects or the request times out.
    /// </summary>
    protected CancellationToken CancellationToken => HttpContext.RequestAborted;
}
