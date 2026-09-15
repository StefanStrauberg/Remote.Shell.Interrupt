using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
using Remote.Shell.Interrupt.Storehouse.Application.Helpers;

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

        // docker-compose.yml/.env.example bake in a public placeholder password so the stack
        // runs out of the box locally. It passes the check above (it's non-empty), so without
        // this it would seed - or on a later restart, silently keep - an administrator account
        // anyone can log into with a password documented in the README. Checked on every
        // startup, not just first-run creation, so a later environment flip to Production is
        // still caught even though the account already exists.
        var environment = provider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment() && adminPassword == InsecureDefaults.DevAdminPassword)
            throw new InvalidOperationException(
                "IdentitySeed:AdminPassword is still the public development placeholder from docker-compose.yml/.env.example. " +
                "Set ADMIN_PASSWORD (or IdentitySeed__AdminPassword) to a real password before running outside Development.");

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            // See the matching comment in IdentityService.RegisterAsync: accounts in this
            // system are provisioned by an operator (here, at startup from configuration),
            // never via self-service sign-up, so there is no confirmation email to send.
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
