using Microsoft.AspNetCore.Diagnostics;

namespace Bilionn_backend.Exceptions;

public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> loggerrr;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        loggerrr = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
