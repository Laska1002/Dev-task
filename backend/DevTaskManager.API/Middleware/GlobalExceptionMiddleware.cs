using System.Net;
using System.Text.Json;

namespace DevTaskManager.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new { message = exception.Message };
        
        // Basic distinction between validation errors (400) and server errors (500)
        // In a real app we might check for specific exception types (e.g. ValidationException)
        if (exception.Message.Contains("inválid") || exception.Message.Contains("registrad") || exception.Message.Contains("en uso"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception.Message.Contains("no encontrad"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response = new { message = "An internal server error occurred." };
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
