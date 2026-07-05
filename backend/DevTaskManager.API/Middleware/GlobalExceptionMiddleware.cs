using System.Net;
using System.Text.Json;
using DevTaskManager.Domain.Exceptions;

namespace DevTaskManager.API.Middleware;

/// <summary>
/// Middleware de manejo global de excepciones.
/// </summary>
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
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, message, errors) = exception switch
        {
            NotFoundException ex       => (HttpStatusCode.NotFound,           ex.Message, (IReadOnlyList<string>?)null),
            BusinessValidationException ex => (HttpStatusCode.BadRequest,     ex.Message, ex.Errors),
            UnauthorizedException ex   => (HttpStatusCode.Unauthorized,       ex.Message, null),
            _                          => (HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.", null)
        };

        // Log detallado solo para errores 500
        context.Response.StatusCode = (int)statusCode;

        object response = errors != null
            ? new { message, errors }
            : new { message };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
