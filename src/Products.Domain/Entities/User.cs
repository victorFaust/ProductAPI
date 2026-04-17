namespace Products.Domain.Entities;

public  class User
{
    public Guid Id { get;  set; }
    public string Username { get;  set; } = string.Empty;
    public string PasswordHash { get;  set; } = string.Empty;
    public string[] Roles { get;  set; } = Array.Empty<string>();
    public DateTime CreatedAt { get;  set; }

    private User() { }

    public static User Create(string username, string passwordHash, string[] roles) =>
        new()
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = passwordHash,
            Roles = roles,
            CreatedAt = DateTime.UtcNow
        };
}
