using System.Net;
using System.Net.Http.Json;
using Tests.Integration.Fixtures;

namespace Tests.Integration;

/// <summary>
/// Regression coverage for reportError.ts actually forwarding to the backend (instead of only
/// console.error) - see ReportClientErrorCommand. The endpoint must work anonymously (a crash
/// can happen before the user is authenticated, e.g. on the login page) and must not let an
/// anonymous caller write to the log file without limit.
/// </summary>
[Collection("Integration")]
public class DiagnosticsEndToEndTests(ApiFactory factory)
{
  static object Payload(string message = "Something broke", string? context = null) => new
  {
    message,
    stack = "at foo (app.js:1:1)",
    url = "https://app.example/workflows/1",
    userAgent = "Mozilla/5.0",
    context
  };

  [Fact]
  public async Task ReportClientError_WithoutAuthentication_ReturnsOk()
  {
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/v1/Diagnostics/ReportClientError", Payload());

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task ReportClientError_EmptyMessage_ReturnsUnprocessableEntity()
  {
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/v1/Diagnostics/ReportClientError", Payload(message: ""));

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task ReportClientError_BurstFromOneClient_IsEventuallyRateLimited()
  {
    using var client = factory.CreateClient();

    HttpStatusCode? rejected = null;
    for (var i = 0; i < 30 && rejected is null; i++)
    {
      var response = await client.PostAsJsonAsync("/api/v1/Diagnostics/ReportClientError", Payload());
      if (response.StatusCode == HttpStatusCode.TooManyRequests)
        rejected = response.StatusCode;
    }

    rejected.Should().Be(HttpStatusCode.TooManyRequests);
  }
}
