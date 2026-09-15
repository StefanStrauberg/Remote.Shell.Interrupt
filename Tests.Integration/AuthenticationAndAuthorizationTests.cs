using System.Net;
using System.Net.Http.Json;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Tests.Integration.Fixtures;
using Tests.Integration.Helpers;

namespace Tests.Integration;

/// <summary>
/// Drives the real JWT bearer + ASP.NET Core Identity + role-authorization pipeline through
/// actual HTTP calls against a real PostgreSQL-backed Identity store - none of this (token
/// issuance, signature validation, the global "authenticated by default" fallback policy,
/// [Authorize(Roles = "Admin")] enforcement) was covered end-to-end before, only in isolation
/// via mocked handlers.
/// </summary>
[Collection("Integration")]
public class AuthenticationAndAuthorizationTests(ApiFactory apiFactory)
{
  readonly ApiFactory _apiFactory = apiFactory;

  [Fact]
  public async Task ProtectedEndpoint_NoToken_ReturnsUnauthorized()
  {
    var client = _apiFactory.CreateClient();

    var response = await client.GetAsync("/api/v1/Gates/GetGatesByFilter");

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task Login_ValidAdminCredentials_ReturnsJwtThatAuthorizesAdminOnlyEndpoint()
  {
    var client = _apiFactory.CreateClient();

    var token = await client.LoginAsAdminAsync();
    client.UseBearerToken(token);
    var response = await client.GetAsync("/api/v1/Gates/GetGatesByFilter");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task Login_WrongPassword_ReturnsUnauthorizedWithoutLeakingWhyItFailed()
  {
    var client = _apiFactory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/v1/Auth/Login",
      new { Email = ApiFactory.AdminEmail, Password = "definitely-wrong" });

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  // Uppercase + lowercase + digit + non-alphanumeric: ASP.NET Core Identity's default
  // password complexity rules (never relaxed by this app's AddIdentityServices), unlike the
  // seeded admin password which happens to satisfy them incidentally.
  const string NonAdminPassword = "A-strong-password-1";

  [Fact]
  public async Task NonAdminUser_CallsAdminOnlyEndpoint_ReturnsForbidden()
  {
    var client = _apiFactory.CreateClient();
    var userToken = await client.RegisterAndLoginAsync(
      $"user-{Guid.NewGuid():N}@integration.test", NonAdminPassword, "User");
    client.UseBearerToken(userToken);

    var response = await client.GetAsync("/api/v1/Gates/GetGatesByFilter");

    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task NonAdminUser_CanReachEndpointsSharedByBothRoles()
  {
    var client = _apiFactory.CreateClient();
    var userToken = await client.RegisterAndLoginAsync(
      $"user-{Guid.NewGuid():N}@integration.test", NonAdminPassword, "User");
    client.UseBearerToken(userToken);

    var response = await client.GetAsync("/api/v1/Clients/GetClientsByFilter");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task NonAdminUser_CannotRegisterNewAccounts()
  {
    var client = _apiFactory.CreateClient();
    var userToken = await client.RegisterAndLoginAsync(
      $"user-{Guid.NewGuid():N}@integration.test", NonAdminPassword, "User");
    client.UseBearerToken(userToken);

    var response = await client.PostAsJsonAsync("/api/v1/Auth/Register",
      new { Email = $"nested-{Guid.NewGuid():N}@integration.test", Password = NonAdminPassword, Role = "User" });

    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CookieLogin_ValidCredentials_EstablishesSessionCookieThatAuthorizesRequests()
  {
    // TestServer's in-memory transport bypasses HttpClientHandler's own cookie container, so
    // the round trip is done by hand: read Set-Cookie from the login response, replay it as a
    // Cookie header on subsequent requests - exactly what a browser would do automatically.
    var client = _apiFactory.CreateClient();

    var loginResponse = await client.PostAsJsonAsync("/api/v1/Auth/CookieLogin",
      new CookieLoginRequest(ApiFactory.AdminEmail, ApiFactory.AdminPassword));
    loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    var authCookie = ExtractCookie(loginResponse, "rsi.auth");
    authCookie.Should().NotBeNull();

    using var gatesRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Gates/GetGatesByFilter");
    gatesRequest.Headers.Add("Cookie", authCookie);
    var gatesResponse = await client.SendAsync(gatesRequest);
    gatesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    using var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Auth/CookieLogout");
    logoutRequest.Headers.Add("Cookie", authCookie);
    var logoutResponse = await client.SendAsync(logoutRequest);
    logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  static string? ExtractCookie(HttpResponseMessage response, string cookieName)
  {
    if (!response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
      return null;

    return setCookieHeaders.FirstOrDefault(h => h.StartsWith($"{cookieName}=", StringComparison.Ordinal))
                          ?.Split(';', 2)[0];
  }
}
