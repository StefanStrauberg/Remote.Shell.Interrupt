using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
    // UseSerilog (not the narrower Logging.AddSerilog) also registers DiagnosticContext,
    // which UseSerilogRequestLogging's middleware requires to construct at all - without it,
    // the very first time the middleware pipeline is built (on host startup), DI throws and
    // the app never starts serving requests.
    builder.Host.UseSerilog(Log.Logger);

    // Identity & authentication
    builder.Services.AddIdentityServices(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddAuthenticationAndAuthorization(builder.Configuration, builder.Environment.IsDevelopment());

    // Application Layers
    builder.Services.AddApplicationServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddWorkflowEngineServices();
    builder.Services.AddSpecificationServices();
    builder.Services.AddQueryFilterParserServices();
    builder.Services.AddPersistenceServices(builder.Configuration);

    // API Infrastructure
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGenWithBearerAuth();

    // Cross-cutting concerns
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    var isDevelopment = builder.Environment.IsDevelopment();

    builder.Services.AddCors(options =>
                    {
                      options.AddPolicy(DefaultEntities.CorsPolicyName, corsPolicy =>
                      {
                        // The SPA's session is an HttpOnly cookie (see AuthController.CookieLogin),
                        // so a cross-origin request needs the browser to accept a cross-origin
                        // Set-Cookie and later send it back - which requires AllowCredentials()
                        // here, paired with a concrete origin: wildcard and AllowCredentials() are
                        // mutually exclusive by the CORS spec (and ASP.NET Core's policy builder
                        // throws if both are configured), so AllowAnyOrigin() can never be combined
                        // with it.
                        var allowsCredentialedOrigin = allowedOrigins.Length > 0 || isDevelopment;

                        if (allowedOrigins.Length > 0)
                          corsPolicy.WithOrigins(allowedOrigins);
                        else if (isDevelopment)
                          // `npm run dev` serves the SPA on :3000 against the API on :5000 (see
                          // client/vite.config.ts and client/.env.development) - that's the
                          // "Development, no explicit origins configured" case this covers.
                          corsPolicy.WithOrigins("http://localhost:3000");
                        // Outside Development, with no configured origins, no origin is allowed:
                        // safer default than permitting any site to call the API cross-origin.

                        if (allowsCredentialedOrigin)
                          corsPolicy.AllowCredentials();

                        corsPolicy.AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .WithExposedHeaders(DefaultEntities.ExposedHeaders);
                      });
                    });

    builder.Services.AddAuthRateLimiting();

    builder.Services.AddScoped<CorrelationIdMiddleware>();
  }

  /// <summary>
  /// Registers Swagger generation with a JWT bearer security scheme, so the "Authorize"
  /// button in Swagger UI can attach an access token (obtained from /api/v1/Auth/Login or
  /// /api/v1/Auth/RefreshToken) to every "Try it out" request. Without this, Swagger has no
  /// notion of authentication and every protected endpoint can only be exercised with
  /// hand-crafted requests outside the UI.
  /// </summary>
  static IServiceCollection AddSwaggerGenWithBearerAuth(this IServiceCollection services)
  {
    services.AddSwaggerGen(options =>
    {
      const string schemeId = "Bearer";

      options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
      {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the access token from POST /api/v1/Auth/Login or /api/v1/Auth/RefreshToken. " +
                      "Swagger adds the \"Bearer \" prefix automatically."
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = schemeId
            }
          },
          []
        }
      });
    });

    return services;
  }

  /// <summary>
  /// Registers a rate-limiting policy for credential-checking auth endpoints
  /// (login, cookie login), partitioned per client IP address, to slow down
  /// brute-force/credential-stuffing attempts against those endpoints.
  /// </summary>
  static IServiceCollection AddAuthRateLimiting(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

      options.OnRejected = async (context, cancellationToken) =>
      {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
          context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

        context.HttpContext.Response.ContentType = "application/json";

        var response = ApiErrorResponse.CreateGenericError(
          StatusCodes.Status429TooManyRequests,
          "Too many attempts. Please try again later.");

        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);
      };

      options.AddPolicy(DefaultEntities.AuthRateLimitPolicy, httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
          partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
          factory: _ => new SlidingWindowRateLimiterOptions
          {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 4,
            QueueLimit = 0
          }));
    });

    return services;
  }

  /// <summary>
  /// Configures the authentication handlers (JWT bearer + Identity cookie behind a
  /// policy scheme) and the fallback authorization policy that requires an
  /// authenticated user for every endpoint unless it is marked [AllowAnonymous].
  /// </summary>
  public static IServiceCollection AddAuthenticationAndAuthorization(
    this IServiceCollection services,
    IConfiguration configuration,
    bool isDevelopment)
  {
    var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                      ?? throw new InvalidOperationException(
                        $"Configuration section '{JwtSettings.SectionName}' is missing.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
      throw new InvalidOperationException(
        $"JwtSettings:Key is missing or shorter than 32 characters. " +
        "Supply it via user-secrets or the JwtSettings__Key environment variable.");

    // docker-compose.yml/.env.example bake in a public placeholder key so the stack runs
    // out of the box locally; it satisfies the length check above but must never sign real
    // tokens. Refuse to start rather than silently issuing JWTs anyone can forge.
    if (!isDevelopment && jwtSettings.Key == InsecureDefaults.DevJwtKey)
      throw new InvalidOperationException(
        "JwtSettings:Key is still the public development placeholder from docker-compose.yml/.env.example. " +
        "Set JWT_KEY (or JwtSettings__Key) to a real, secret signing key before running outside Development.");

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
        options.Cookie.SecurePolicy = isDevelopment
          ? CookieSecurePolicy.SameAsRequest
          : CookieSecurePolicy.Always;
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
    // Registered first so the correlation ID it pushes onto Serilog's log context covers
    // every log line written further down the pipeline - including the request-logging
    // summary below and everything ExceptionHandlingMiddleware or a handler logs - letting
    // them all be tied back to the same request when investigating an incident.
    app.UseMiddleware<CorrelationIdMiddleware>();

    // Registered next so the logged duration and status code cover the entire
    // downstream pipeline, including exceptions turned into error responses by
    // ExceptionHandlingMiddleware and 429s from the rate limiter.
    app.UseSerilogRequestLogging(options =>
    {
      // An orchestrator polls /health/live and /health/ready every few seconds; logging
      // each of those at Information would drown out everything else. They still show up
      // if they ever fail (Warning), or error (Error) - only the routine "still healthy"
      // case is quieted down, and only below the configured Information minimum level.
      options.GetLevel = (httpContext, elapsedMs, ex) => ex is not null
        ? Serilog.Events.LogEventLevel.Error
        : httpContext.Response.StatusCode > 499
          ? Serilog.Events.LogEventLevel.Error
          : httpContext.Request.Path.StartsWithSegments("/health") && httpContext.Response.StatusCode < 400
            ? Serilog.Events.LogEventLevel.Verbose
            : Serilog.Events.LogEventLevel.Information;
    });

    // Registered first so it also catches exceptions thrown by CORS/authentication/
    // authorization middleware further down the pipeline, not just controller actions.
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors(DefaultEntities.CorsPolicyName);

    app.UseRateLimiter();

    // Registered before authentication/authorization: Swagger has no [AllowAnonymous]
    // metadata to opt out with, and the global fallback policy below requires an
    // authenticated user for every request that isn't explicitly exempted — including
    // ones that never mapped to a controller endpoint. Placed here, this middleware
    // fully handles and terminates matching /swagger/* requests itself, so they never
    // reach the authorization check at all.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecksEndpoints();
  }

  /// <summary>
  /// Maps liveness/readiness endpoints for deployment behind a load balancer or
  /// orchestrator (e.g. Kubernetes): "/health/live" only confirms the process itself is
  /// responding - it never depends on PostgreSQL or the remote MySQL billing database,
  /// so a transient database outage doesn't make an orchestrator kill and restart a
  /// perfectly healthy instance. "/health/ready" runs both database checks and is what
  /// should gate whether traffic gets routed to this instance. "/health" runs everything,
  /// for a quick manual check. All three are anonymous: the caller is a load balancer or
  /// orchestrator, not an authenticated user.
  /// </summary>
  static void MapHealthChecksEndpoints(this WebApplication app)
  {
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
      ResponseWriter = HealthCheckResponseWriter.WriteAsync
    }).AllowAnonymous();

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
      Predicate = check => check.Tags.Contains("ready"),
      ResponseWriter = HealthCheckResponseWriter.WriteAsync
    }).AllowAnonymous();

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
      Predicate = _ => false,
      ResponseWriter = HealthCheckResponseWriter.WriteAsync
    }).AllowAnonymous();
  }
}
