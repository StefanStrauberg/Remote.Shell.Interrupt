using System.Net;
using System.Text.Json;
using Tests.Integration.Fixtures;

namespace Tests.Integration;

[Collection("Integration")]
public class HealthCheckEndpointsTests(ApiFactory apiFactory)
{
  readonly HttpClient _client = apiFactory.CreateClient();

  [Fact]
  public async Task HealthLive_NeverTouchesDatabases_ReturnsHealthyImmediately()
  {
    var response = await _client.GetAsync("/health/live");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    json.RootElement.GetProperty("checks").GetArrayLength().Should().Be(0);
  }

  [Fact]
  public async Task HealthLive_IsAnonymous_NoAuthorizationHeaderRequired()
  {
    var response = await _client.GetAsync("/health/live");

    response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task HealthReady_BothRealDatabasesReachable_ReportsBothChecksHealthy()
  {
    var response = await _client.GetAsync("/health/ready");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    var checks = json.RootElement.GetProperty("checks").EnumerateArray().ToList();

    checks.Should().Contain(c => c.GetProperty("name").GetString() == "postgresql"
                               && c.GetProperty("status").GetString() == "Healthy");
    checks.Should().Contain(c => c.GetProperty("name").GetString() == "mysql-billing"
                               && c.GetProperty("status").GetString() == "Healthy");
  }

  [Fact]
  public async Task Health_ReturnsAllChecksCombined()
  {
    var response = await _client.GetAsync("/health");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    json.RootElement.GetProperty("checks").GetArrayLength().Should().Be(2);
  }
}
