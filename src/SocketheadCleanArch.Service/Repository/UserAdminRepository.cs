using Microsoft.AspNetCore.Identity;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Service.Repository;

public class UserAdminRepository(
    UserManager<AppUser> userManager, 
    RoleManager<AppRole> roleManager)
{
    public IQueryable<AppUser> Users => userManager.Users;
    public IQueryable<AppRole> Roles => roleManager.Roles;

    public async Task<AppUser?> FindUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<AppUser?> FindUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
    
    public async Task<IReadOnlyList<string>> GetUserRolesAsync(AppUser user)
    {
        return (await userManager.GetRolesAsync(user)).ToArray();
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        return await roleManager.CreateAsync(new AppRole(roleName));
    }
    public async Task<AppUser?> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return await userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    public async Task<IdentityResult> CreateUserAsync(AppUser user)
    {
        return await userManager.CreateAsync(user);
    }

    public async Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo loginInfo)
    {
        return await userManager.AddLoginAsync(user, loginInfo);
    }

    public async Task<bool> AuthenticateUserAsync(AppUser user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }
    
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