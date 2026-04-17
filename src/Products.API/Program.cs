using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Products.Application;
using Products.Application.Common;
using Products.API;
using Products.API.Middleware;
using Products.Domain.Entities;
using Products.Infrastructure;
using Products.Infrastructure.Persistence.Context;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Products API");

    var builder = WebApplication.CreateBuilder(args);

    // Remove the "Server: Kestrel" response header
    builder.WebHost.ConfigureKestrel(k => k.AddServerHeader = false);

    builder.Host.UseSerilog((ctx, services, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext());

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
        options.AddPolicy("Frontend", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token (without 'Bearer ' prefix)"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // Apply pending migrations on startup 
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (db.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();

        // Seed default users if none exist
        if (!db.Users.Any())
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var seedUsers = config.GetSection("SeedUsers").Get<SeedUserOptions[]>()
                ?? Array.Empty<SeedUserOptions>();

            db.Users.AddRange(seedUsers.Select(u =>
                User.Create(u.Username, hasher.Hash(u.Password), u.Roles)));

            db.SaveChanges();
        }
    }

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseCors("Frontend");

    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1"));

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) 
{
    Log.Fatal(ex, "Products API terminated unexpectedly");
}

public partial class Program { }

