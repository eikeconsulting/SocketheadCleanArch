using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Admin.MFA;

public class AdditionalUserClaimsPrincipalFactory(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor) :
    UserClaimsPrincipalFactory<AppUser, AppRole>(userManager, roleManager, optionsAccessor)
{
    public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
    {
        ClaimsPrincipal principal = await base.CreateAsync(user);
        ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;
        identity?.AddClaim(new Claim("amr", user.TwoFactorEnabled ? "mfa" : "pwd"));
        return principal;
    }
}