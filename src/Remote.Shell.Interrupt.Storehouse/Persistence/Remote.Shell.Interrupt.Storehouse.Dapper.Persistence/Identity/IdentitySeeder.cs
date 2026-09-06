using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// Idempotent startup seeding: ensures the "Admin" and "User" roles exist and
/// provisions the default administrator account from configuration.
///
/// The administrator credentials are read from the "IdentitySeed" section
/// (AdminEmail/AdminPassword). When the password is not configured the admin
/// account is skipped with a warning, so production deployments must supply
/// it through user-secrets or environment variables.
/// </summary>
public sealed class IdentitySeeder
{
    const string AdminRole = "Admin";
    const string UserRole = "User";

    public static async Task SeedIdentityAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var logger = provider.GetRequiredService<IAppLogger<IdentitySeeder>>();

        foreach (var roleName in (string[]) [AdminRole, UserRole])
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

            if (!createRoleResult.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create identity role '{roleName}': " +
                    string.Join("; ", createRoleResult.Errors.Select(error => error.Description)));

            logger.LogInformation("Created identity role '{RoleName}'.", roleName);
        }

        var adminEmail = configuration["IdentitySeed:AdminEmail"];
        var adminPassword = configuration["IdentitySeed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning(
                "IdentitySeed:AdminEmail/AdminPassword are not configured; " +
                "the default administrator account was not provisioned.");
            return;
        }

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator",
            CreatedAtUtc = DateTime.UtcNow
        };

        var createUserResult = await userManager.CreateAsync(admin, adminPassword);

        if (!createUserResult.Succeeded)
            throw new InvalidOperationException(
                "Failed to provision the default administrator account: " +
                string.Join("; ", createUserResult.Errors.Select(error => error.Description)));

        var addToRoleResult = await userManager.AddToRoleAsync(admin, AdminRole);

        if (!addToRoleResult.Succeeded)
            throw new InvalidOperationException(
                "Failed to assign the Admin role to the default administrator: " +
                string.Join("; ", addToRoleResult.Errors.Select(error => error.Description)));

        logger.LogInformation("Provisioned the default administrator account '{AdminEmail}'.", adminEmail);
    }
}
