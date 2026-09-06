using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.API.Services;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Remote.Shell.Interrupt.Storehouse.API;

/// <summary>
/// Provides extension methods for registering services and configuring middleware in the application pipeline.
/// </summary>
public static class ServiceRegistration
{
  /// <summary>
  /// Policy scheme that selects the authentication handler per request:
  /// JWT bearer when an Authorization header is present, Identity cookie otherwise.
  /// </summary>
  public const string SmartAuthenticationScheme = "SmartAuthentication";

  /// <summary>
  /// Configures dependency injection for core application components, infrastructure services, and cross-cutting concerns.
  /// </summary>
  /// <param name="builder">The application builder used to register services during startup.</param>
  public static void AddApplicationServices(this WebApplicationBuilder builder)
  {
    // Logging
    builder.Services.AddLoggerServices();
    builder.Logging.AddSerilog(Log.Logger);

    // Identity & authentication
    builder.Services.AddIdentityServices(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

    // Application Layers
    builder.Services.AddApplicationServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddSpecificationServices();
    builder.Services.AddQueryFilterParserServices();
    builder.Services.AddPersistenceServices(builder.Configuration);

    // API Infrastructure
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Cross-cutting concerns
    builder.Services.AddCors(options =>
                    {
                      options.AddPolicy(DefaultEntities.CorsPolicyName,
                                        builder => builder.AllowAnyOrigin()
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod()
                                                          .WithExposedHeaders(DefaultEntities.ExposedHeaders));
                    });
  }

  /// <summary>
  /// Configures the authentication handlers (JWT bearer + Identity cookie behind a
  /// policy scheme) and the fallback authorization policy that requires an
  /// authenticated user for every endpoint unless it is marked [AllowAnonymous].
  /// </summary>
  public static IServiceCollection AddAuthenticationAndAuthorization(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                      ?? throw new InvalidOperationException(
                        $"Configuration section '{JwtSettings.SectionName}' is missing.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
      throw new InvalidOperationException(
        $"JwtSettings:Key is missing or shorter than 32 characters. " +
        "Supply it via user-secrets or the JwtSettings__Key environment variable.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
      throw new InvalidOperationException("JwtSettings:Issuer and JwtSettings:Audience must be configured.");

    services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = SmartAuthenticationScheme;
        options.DefaultChallengeScheme = SmartAuthenticationScheme;
        options.DefaultScheme = SmartAuthenticationScheme;
      })
      .AddPolicyScheme(SmartAuthenticationScheme, "JWT or Cookie", options =>
      {
        options.ForwardDefaultSelector = context =>
          context.Request.Headers.ContainsKey("Authorization")
            ? JwtBearerDefaults.AuthenticationScheme
            : IdentityConstants.ApplicationScheme;
      })
      .AddJwtBearer(options =>
      {
        // Keep claim types exactly as issued ("sub", "email", "role")
        // instead of letting the handler map them to SOAP/WS-Federation names.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = jwtSettings.Issuer,

          ValidateAudience = true,
          ValidAudience = jwtSettings.Audience,

          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

          ValidateLifetime = true,
          ClockSkew = TimeSpan.FromSeconds(30),

          NameClaimType = JwtRegisteredClaimNames.Sub,
          RoleClaimType = JwtSettings.RoleClaimType
        };
      })
      .AddCookie(IdentityConstants.ApplicationScheme, options =>
      {
        options.Cookie.Name = "rsi.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(jwtSettings.CookieExpiryDays);
        options.SlidingExpiration = true;

        // API semantics: never redirect to a login page, return status codes.
        options.Events.OnRedirectToLogin = context =>
        {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
          context.Response.StatusCode = StatusCodes.Status403Forbidden;
          return Task.CompletedTask;
        };
      });

    // Global authorization policy: everything requires authentication by default.
    // Individual endpoints opt out with [AllowAnonymous] or opt into roles with
    // [Authorize(Roles = "Admin")].
    services.AddAuthorization(options =>
    {
      options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    });

    return services;
  }

  /// <summary>
  /// Configures middleware components for the application, including CORS, Swagger UI, and global exception handling.
  /// </summary>
  /// <param name="app">The web application instance to configure.</param>
  public static void ConfigurePipeline(this WebApplication app)
  {
    app.UseCors(DefaultEntities.CorsPolicyName);

    app.UseAuthentication();
    app.UseAuthorization();

    // Development-specific middleware
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    // Application middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();
  }
}
