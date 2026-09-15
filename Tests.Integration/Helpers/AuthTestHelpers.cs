using System.Net.Http.Headers;
using System.Net.Http.Json;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Register;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Tests.Integration.Fixtures;

namespace Tests.Integration.Helpers;

public static class AuthTestHelpers
{
  /// <summary>
  /// Logs in as the seeded default administrator (see <see cref="ApiFactory.AdminEmail"/>)
  /// and returns the issued JWT access token.
  /// </summary>
  public static async Task<string> LoginAsAdminAsync(this HttpClient client)
  {
    var response = await client.PostAsJsonAsync("/api/v1/Auth/Login",
      new LoginCommand(ApiFactory.AdminEmail, ApiFactory.AdminPassword));

    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();

    return result!.Token!;
  }

  /// <summary>
  /// Registers a new account with the given role (as the admin) and logs in as it,
  /// returning the issued JWT access token for a non-admin caller.
  /// </summary>
  public static async Task<string> RegisterAndLoginAsync(this HttpClient client, string email, string password, string role)
  {
    var adminToken = await client.LoginAsAdminAsync();

    using var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Auth/Register")
    {
      Content = JsonContent.Create(new RegisterCommand(email, password, role))
    };
    registerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

    var registerResponse = await client.SendAsync(registerRequest);
    registerResponse.EnsureSuccessStatusCode();

    var loginResponse = await client.PostAsJsonAsync("/api/v1/Auth/Login", new LoginCommand(email, password));
    loginResponse.EnsureSuccessStatusCode();
    var result = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResult>();

    return result!.Token!;
  }

  public static void UseBearerToken(this HttpClient client, string token)
    => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
