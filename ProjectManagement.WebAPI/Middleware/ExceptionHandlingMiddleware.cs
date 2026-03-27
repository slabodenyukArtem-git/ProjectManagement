using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ProjectManagement.WebAPI.Middleware;

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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        object response;
        
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            response = new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            };
        }
        else if (exception is Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            response = new
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = "Database error",
                Detail = dbEx.InnerException?.Message ?? dbEx.Message
            };
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occurred while processing your request",
                Detail = exception.Message
            };
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
