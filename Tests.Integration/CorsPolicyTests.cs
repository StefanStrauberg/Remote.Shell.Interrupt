using System.Net;
using Tests.Integration.Fixtures;

namespace Tests.Integration;

/// <summary>
/// Regression coverage for the CORS policy in ServiceRegistration.cs: it must never combine a
/// wildcard origin with AllowCredentials() (the classic "any site can mint an authenticated
/// cookie session against this API" misconfiguration), since the SPA's session is an HttpOnly
/// cookie that depends on AllowCredentials() being paired with a concrete, checked origin. The
/// fixture runs with ASPNETCORE_ENVIRONMENT=Development and no Cors:AllowedOrigins configured
/// (see ApiFactory), which is the "no explicit origins configured, still Development" branch
/// that pins to http://localhost:3000 (matching client/vite.config.ts's dev server port).
/// </summary>
[Collection("Integration")]
public class CorsPolicyTests(ApiFactory apiFactory)
{
  readonly HttpClient _client = apiFactory.CreateClient();

  [Fact]
  public async Task Request_FromAllowedOrigin_ReflectsOriginAndAllowsCredentials()
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
    request.Headers.Add("Origin", "http://localhost:3000");

    var response = await _client.SendAsync(request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowOrigin).Should().BeTrue();
    allowOrigin!.Should().ContainSingle().Which.Should().Be("http://localhost:3000");

    response.Headers.TryGetValues("Access-Control-Allow-Credentials", out var allowCredentials).Should().BeTrue();
    allowCredentials!.Should().ContainSingle().Which.Should().Be("true");
  }

  [Fact]
  public async Task Request_FromUntrustedOrigin_GetsNoAccessControlAllowOriginHeader()
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
    request.Headers.Add("Origin", "https://evil.example");

    var response = await _client.SendAsync(request);

    // CORS is enforced by the browser reading these response headers, not by the server
    // refusing the request - what matters is the server never tells a browser this origin may
    // read the response or that credentials are allowed for it.
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
    response.Headers.Contains("Access-Control-Allow-Credentials").Should().BeFalse();
  }

  [Fact]
  public async Task PreflightFromAllowedOrigin_AllowsCredentialedCookieLoginRequest()
  {
    using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/Auth/CookieLogin");
    request.Headers.Add("Origin", "http://localhost:3000");
    request.Headers.Add("Access-Control-Request-Method", "POST");
    request.Headers.Add("Access-Control-Request-Headers", "content-type");

    var response = await _client.SendAsync(request);

    response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowOrigin).Should().BeTrue();
    allowOrigin!.Should().ContainSingle().Which.Should().Be("http://localhost:3000");
    response.Headers.TryGetValues("Access-Control-Allow-Credentials", out var allowCredentials).Should().BeTrue();
    allowCredentials!.Should().ContainSingle().Which.Should().Be("true");
  }

  [Fact]
  public async Task PreflightFromUntrustedOrigin_IsNotAllowed()
  {
    using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/Auth/CookieLogin");
    request.Headers.Add("Origin", "https://evil.example");
    request.Headers.Add("Access-Control-Request-Method", "POST");
    request.Headers.Add("Access-Control-Request-Headers", "content-type");

    var response = await _client.SendAsync(request);

    response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
    response.Headers.Contains("Access-Control-Allow-Credentials").Should().BeFalse();
  }
}
