using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Tests.Integration.Fixtures;
using Tests.Integration.Helpers;

namespace Tests.Integration;

/// <summary>
/// Exercises full Gate CRUD through real HTTP calls against a real PostgreSQL database -
/// including a case-insensitive "Contains" filter, which the API rewrites to Npgsql's
/// <c>ILIKE</c> (see ILikeExpressionVisitor). That rewrite was previously only checked by
/// inspecting the resulting expression tree; nothing had ever executed it against an actual
/// Postgres server, which is the only engine that understands <c>ILIKE</c> in the first place.
/// </summary>
[Collection("Integration")]
public class GatesEndToEndPostgresTests(ApiFactory apiFactory)
{
  readonly ApiFactory _apiFactory = apiFactory;

  [Fact]
  public async Task Gate_CreateFilterUpdateDelete_RoundTripsThroughRealPostgres()
  {
    var client = _apiFactory.CreateClient();
    client.UseBearerToken(await client.LoginAsAdminAsync());
    var gateName = $"gw-{Guid.NewGuid():N}";

    // Create
    var createResponse = await client.PostAsJsonAsync("/api/v1/Gates/CreateGate", new CreateGateDTO
    {
      Name = gateName,
      Community = "public",
      IPAddress = "192.168.100.1",
      TypeOfNetworkDevice = "Cisco"
    });
    createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    // Filter: mixed-case substring, must match via a real ILIKE against Postgres.
    var filterQuery = QueryHelpers.AddQueryString("/api/v1/Gates/GetGatesByFilter", new Dictionary<string, string?>
    {
      ["Filters[0].PropertyPath"] = "Name",
      ["Filters[0].Operator"] = "Contains",
      ["Filters[0].Value"] = gateName.ToUpperInvariant()
    });
    var filterResponse = await client.GetAsync(filterQuery);
    filterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    var filtered = await filterResponse.Content.ReadFromJsonAsync<List<GateDTO>>();
    var found = filtered.Should().ContainSingle(g => g.Name == gateName).Subject;

    // Update
    var updateResponse = await client.PutAsJsonAsync("/api/v1/Gates/UpdateGate", new UpdateGateDTO
    {
      Id = found.Id,
      Name = gateName,
      Community = "private",
      IPAddress = "192.168.100.2",
      TypeOfNetworkDevice = "Juniper"
    });
    updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var afterUpdateResponse = await client.GetAsync($"/api/v1/Gates/GetGateById/{found.Id}");
    afterUpdateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    var afterUpdate = await afterUpdateResponse.Content.ReadFromJsonAsync<GateDTO>();
    afterUpdate!.Community.Should().Be("private");
    afterUpdate.IPAddress.Should().Be("192.168.100.2");

    // Delete
    var deleteResponse = await client.DeleteAsync($"/api/v1/Gates/DeleteGateById/{found.Id}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var afterDeleteResponse = await client.GetAsync($"/api/v1/Gates/GetGateById/{found.Id}");
    afterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}
