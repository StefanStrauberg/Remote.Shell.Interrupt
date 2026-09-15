using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remote.Shell.Interrupt.Storehouse.API;
using Remote.Shell.Interrupt.Storehouse.Application.Helpers;

namespace Tests.Api;

public class ServiceRegistrationTests
{
    static IConfiguration BuildConfiguration(string jwtKey)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = jwtKey,
                ["JwtSettings:Issuer"] = "RemoteShellInterrupt.API",
                ["JwtSettings:Audience"] = "RemoteShellInterrupt.Client",
            })
            .Build();

    [Fact]
    public void AddAuthenticationAndAuthorization_DevJwtKeyOutsideDevelopment_Throws()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(InsecureDefaults.DevJwtKey);

        var act = () => services.AddAuthenticationAndAuthorization(configuration, isDevelopment: false);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*development placeholder*");
    }

    [Fact]
    public void AddAuthenticationAndAuthorization_DevJwtKeyInDevelopment_DoesNotThrow()
    {
        // The whole point of the placeholder is that `docker compose up`/`dotnet run` work
        // out of the box in Development - only running outside it with the placeholder still
        // in effect is the misconfiguration this guard exists to catch.
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(InsecureDefaults.DevJwtKey);

        var act = () => services.AddAuthenticationAndAuthorization(configuration, isDevelopment: true);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddAuthenticationAndAuthorization_RealJwtKeyOutsideDevelopment_DoesNotThrow()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration("a-real-secret-signing-key-at-least-32-characters-long");

        var act = () => services.AddAuthenticationAndAuthorization(configuration, isDevelopment: false);

        act.Should().NotThrow();
    }
}
