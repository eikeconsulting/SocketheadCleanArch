using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.Admin.Seeding;

public static class DataSeederExtensions
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedRolesAsync();
        await seeder.SeedUsersAsync();
    }
}

public class DataSeeder(UserAdminRepository repo, IOptions<DataSeedSettings> settings)
{
    public async Task SeedRolesAsync()
    {
        HashSet<string> existingRoles = await repo.Roles
            .Select(r => r.Name!)
            .ToHashSetAsync();
        
        foreach (string role in settings.Value.Roles.Where(r => !existingRoles.Contains(r)))
            await repo.CreateRoleAsync(role);
    }

    public async Task SeedUsersAsync()
    {
        foreach (UserSeed userSeed in settings.Value.Users)
        {
            AppUser? user = await repo.FindUserByEmailAsync(userSeed.Email);
            if (user is null)
            {
                var result = await repo.CreateUserAsync(userSeed.Email, userSeed.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
                user = await repo.FindUserByEmailAsync(userSeed.Email);
            }

            if (user is not null)
                await repo.SetRolesAsync(user, userSeed.Roles.Where(r => !string.IsNullOrEmpty(r)).ToList());
        }
    }
}