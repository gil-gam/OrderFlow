using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);

            var problem = new ValidationProblemDetails(
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = (int)HttpStatusCode.BadRequest
            };

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(context.Response.Body, problem);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized access: {Message}", ex.Message);

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Unauthorized
            };

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(context.Response.Body, problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(context.Response.Body, problem);
        }
    }
}