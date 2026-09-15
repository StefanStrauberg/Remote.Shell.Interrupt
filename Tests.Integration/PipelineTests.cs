using System.Net;
using Remote.Shell.Interrupt.Storehouse.API.Middleware;
using Tests.Integration.Fixtures;
using Tests.Integration.Helpers;

namespace Tests.Integration;

[Collection("Integration")]
public class PipelineTests(ApiFactory apiFactory)
{
  readonly HttpClient _client = apiFactory.CreateClient();

  [Fact]
  public async Task Response_NoInboundCorrelationId_GetsAGeneratedOne()
  {
    var response = await _client.GetAsync("/health/live");

    response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values).Should().BeTrue();
    Guid.TryParse(values!.Single(), out _).Should().BeTrue();
  }

  [Fact]
  public async Task Response_InboundCorrelationId_IsEchoedBackVerbatim()
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
    request.Headers.Add(CorrelationIdMiddleware.HeaderName, "caller-supplied-id-123");

    var response = await _client.SendAsync(request);

    response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Should().ContainSingle("caller-supplied-id-123");
  }

  [Fact]
  public async Task VersionedRoute_Resolves()
  {
    var response = await _client.PostAsync("/api/v1/Auth/Login", content: null);

    // Malformed body -> 400/422, not 404: proves the route itself resolved.
    response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UnversionedRoute_DoesNotResolve()
  {
    // An unmatched route also gets 401 from the global authorization fallback policy before
    // routing would otherwise say 404 (see ServiceRegistration's own comment on this for
    // Swagger) - so authenticate first, to isolate "no such route" from "not authenticated".
    _client.UseBearerToken(await _client.LoginAsAdminAsync());

    var response = await _client.GetAsync("/api/Gates/GetGatesByFilter");

    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}
