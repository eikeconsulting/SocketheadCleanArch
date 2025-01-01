using Microsoft.EntityFrameworkCore;
using SocketheadCleanArch.Domain.Data.Entities;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.Admin;

public static class DataSeederExtensions
{
    public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedRolesAsync();
        await seeder.SeedAdminUsersAsync();
        return app;
    }
}

public class DataSeeder(UserAdminRepository repo)
{
    private readonly string[] Roles = ["Admin", "Developer", "User"];
    
    public async Task SeedRolesAsync()
    {
        HashSet<string> existingRoles = await repo.Roles
            .Select(r => r.Name!)
            .ToHashSetAsync();
        
        foreach (string role in Roles.Where(r => !existingRoles.Contains(r)))
            await repo.CreateRoleAsync(role);
    }

    /// <summary>
    /// TODO: put this in a config file, autogenerate password and have admin recover through email
    /// </summary>
    private readonly string[] adminEmails = [ "admin@foo.com", "admin@bar.com" ];
    private const string adminPassword = "Admin@123!";

    public async Task SeedAdminUsersAsync()
    {
        foreach (string email in adminEmails)
        {
            await repo.CreateUserAsync(email, adminPassword);

            // assuming UserId is Email; perhaps a safe assumption for all projects
            AppUser? user = await repo.FindUserByIdAsync(email);
            if (user != null)
                await repo.SetRolesAsync(user, Roles);
        }
    }
}