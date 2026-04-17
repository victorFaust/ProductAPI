namespace Products.Application.Common.DTOs
{

    public record HealthResponse(string Status, DateTime Timestamp, string Version);

    public record ErrorResponse(string Message);

    public record ValidationErrorResponse(string Message,Dictionary<string, string[]> Errors);
}