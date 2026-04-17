namespace Products.Application.Common.DTOs;

public sealed record HealthResponse(string Status, DateTime Timestamp, string Version);

public sealed record ErrorResponse(string Message);

public sealed record ValidationErrorResponse(
    string Message,
    Dictionary<string, string[]> Errors);
