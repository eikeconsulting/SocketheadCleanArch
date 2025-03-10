using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SocketheadCleanArch.API;

public class ApiExceptionHandler : IExceptionHandler
{
    private static bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    private static bool IsValidationException(Exception exception) => exception
        is System.ComponentModel.DataAnnotations.ValidationException
        or FluentValidation.ValidationException;

    private static string RequestInstance(HttpRequest request) =>
        $"{request.Method} {request.Scheme}://{request.Host}{request.Path}"; 
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string title = "An unexpected error occurred";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        if (IsValidationException(exception))
        {
            title = "Bad Request";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        ProblemDetails details = new()
        {
            Status = httpContext.Response.StatusCode,
            Title = title,
            Detail = exception.Message,
            Type = exception.GetType().Name,
            Instance = RequestInstance(httpContext.Request),
        };

        if (IsDevelopment)
            details.Extensions["StackTrace"] = exception.StackTrace;

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken: cancellationToken);

        return true;
    }
}