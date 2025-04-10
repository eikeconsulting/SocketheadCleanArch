using Microsoft.AspNetCore.Identity;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Service.Repository;

/// <summary>
/// Repository service for managing user and role operations using ASP.NET Identity.
/// </summary>
public class UserAdminRepository(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager)
{
    public IQueryable<AppUser> Users => userManager.Users;
    public IQueryable<AppRole> Roles => roleManager.Roles;

    /// <summary>
    /// Finds a user by their unique user ID.
    /// </summary>
    public async Task<AppUser?> FindUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    public async Task<AppUser?> FindUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Gets all roles assigned to the specified user.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetUserRolesAsync(AppUser user)
    {
        return (await userManager.GetRolesAsync(user)).ToArray();
    }

    /// <summary>
    /// Creates a new role in the identity system.
    /// </summary>
    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        return await roleManager.CreateAsync(new AppRole(roleName));
    }

    /// <summary>
    /// Finds a user associated with an external login provider.
    /// </summary>
    public async Task<AppUser?> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return await userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    /// <summary>
    /// Creates a new user in the identity system without setting a password.
    /// </summary>
    public async Task<IdentityResult> CreateUserAsync(AppUser user)
    {
        return await userManager.CreateAsync(user);
    }

    /// <summary>
    /// Adds an external login to an existing user.
    /// </summary>
    public async Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo loginInfo)
    {
        return await userManager.AddLoginAsync(user, loginInfo);
    }

    /// <summary>
    /// Checks if the provided password is valid for the given user.
    /// </summary>
    public async Task<bool> AuthenticateUserAsync(AppUser user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Sets the specified roles on the user, syncing by adding new ones and removing unassigned ones.
    /// </summary>
    public async Task SetRolesAsync(AppUser user, IReadOnlyList<string> roles)
    {
        IReadOnlyList<string> current = await GetUserRolesAsync(user);
        IReadOnlyList<string> added = roles.Except(current).ToArray();
        IReadOnlyList<string> removed = current.Except(roles).ToArray();

        if (added.Any())
            await userManager.AddToRolesAsync(user, added);

        if (removed.Any())
            await userManager.RemoveFromRolesAsync(user, removed);
    }

    /// <summary>
    /// Creates a new user with the provided email and password.
    /// </summary>
    public async Task<IdentityResult> CreateUserAsync(string email, string password)
    {
        AppUser user = new()
        {
            Id = Guid.NewGuid().ToString(), // RJE: I believe we should not be doing this
            UserName = email,
            Email = email,
        };

        return await userManager.CreateAsync(user, password);
    }
}