using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
using Remote.Shell.Interrupt.Storehouse.Application.Helpers;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Tests.Persistence;

public class IdentitySeederTests
{
    const string AdminEmail = "admin@localhost.local";

    static IServiceProvider BuildProvider(string environmentName, string adminPassword)
    {
        var roleManager = Substitute.For<RoleManager<IdentityRole<Guid>>>(
            Substitute.For<IRoleStore<IdentityRole<Guid>>>(), null, null, null, null);
        roleManager.RoleExistsAsync(Arg.Any<string>()).Returns(true);

        var userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        userManager.FindByEmailAsync(AdminEmail).Returns((ApplicationUser?)null);
        userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["IdentitySeed:AdminEmail"] = AdminEmail,
                ["IdentitySeed:AdminPassword"] = adminPassword,
            })
            .Build();

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.EnvironmentName = environmentName;

        var services = new ServiceCollection();
        services.AddSingleton(roleManager);
        services.AddSingleton(userManager);
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(Substitute.For<IAppLogger<IdentitySeeder>>());
        services.AddSingleton(hostEnvironment);

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task SeedIdentityAsync_DevAdminPasswordOutsideDevelopment_Throws()
    {
        var provider = BuildProvider(Environments.Production, InsecureDefaults.DevAdminPassword);

        var act = async () => await IdentitySeeder.SeedIdentityAsync(provider);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*development placeholder*");
    }

    [Fact]
    public async Task SeedIdentityAsync_DevAdminPasswordInDevelopment_DoesNotThrow()
    {
        // Mirrors the JwtSettings:Key guard: the placeholder exists precisely so Development
        // works with zero manual setup - only carrying it into a non-Development environment
        // is the misconfiguration this guard exists to catch.
        var provider = BuildProvider(Environments.Development, InsecureDefaults.DevAdminPassword);

        var act = async () => await IdentitySeeder.SeedIdentityAsync(provider);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SeedIdentityAsync_RealAdminPasswordOutsideDevelopment_DoesNotThrow()
    {
        var provider = BuildProvider(Environments.Production, "a-real-operator-chosen-password");

        var act = async () => await IdentitySeeder.SeedIdentityAsync(provider);

        await act.Should().NotThrowAsync();
    }
}
