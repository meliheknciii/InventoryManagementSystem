using System.Net;
using System.Text.Json;

namespace InventoryManagementSystem.Middleware;

/// <summary>
/// Translates exceptions thrown by the Application layer into consistent
/// HTTP problem responses, keeping controllers free of try/catch blocks.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Operation not allowed"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception occurred while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            title,
            status = (int)statusCode,
            detail = exception.Message,
            instance = context.Request.Path.Value
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
