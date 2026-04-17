using FluentValidation;
using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.Domain.Exceptions;

namespace Products.API.Middleware;

public class ExceptionMiddleware
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
            _logger.LogWarning(ex, "Validation failed for request {Path}", context.Request.Path);
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            var body = new ValidationErrorResponse("Validation failed.", errors);
            await WriteAsync(context, 400, "Validation failed.", body);
        }
        catch (AuthException ex)
        {
            _logger.LogWarning(ex, "Auth failure for request {Path}", context.Request.Path);
            await WriteAsync(context, 401, ex.Message, new AuthErrorResponse(ex.Message));
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found: {Message}", ex.Message);
            await WriteAsync(context, 404, ex.Message, new ErrorResponse(ex.Message));
        }
        catch (InvalidProductException ex)
        {
            _logger.LogWarning(ex, "Invalid product data: {Message}", ex.Message);
            await WriteAsync(context, 422, ex.Message, new ErrorResponse(ex.Message));
        }
        catch (DuplicateProductException ex)
        {
            _logger.LogWarning(ex, "Duplicate product: {Message}", ex.Message);
            await WriteAsync(context, 409, ex.Message, new ErrorResponse(ex.Message));
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Request cancelled by client: {Path}", context.Request.Path);
            await WriteAsync(context, 499, "The request was cancelled by the client.",
                new ErrorResponse("The request was cancelled by the client."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request {Path}", context.Request.Path);
            await WriteAsync(context, 500, "An unexpected error occurred.",
                new ErrorResponse("An unexpected error occurred."));
        }
    }

    private static async Task WriteAsync<T>(HttpContext context, int statusCode, string message, T data)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<T>.Fail(message, statusCode, data);
        await context.Response.WriteAsJsonAsync(response);
    }
}
