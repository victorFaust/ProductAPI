namespace Products.Infrastructure.Identity;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public int ExpiryMinutes { get; init; }
}
