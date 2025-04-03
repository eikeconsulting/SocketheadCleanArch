using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SocketheadCleanArch.API.Models;
using SocketheadCleanArch.Domain.Dtos;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.API.Authentication;

public class UserAuthService(UserAdminRepository userAdmin, JwtTokenService jwtTokenService)
{
    public async Task<AuthResponse> AuthenticateUserAsync(string email, string password)
    {
        AppUser? user = await userAdmin.FindUserByEmailAsync(email);
        if (user == null)
            return new AuthResponse { FailureReason = "User does not exist or invalid credentials" };
        
        bool success = await userAdmin.AuthenticateUserAsync(user, password);
        if (!success)
            return new AuthResponse { FailureReason = "User does not exist or invalid credentials" };

        IReadOnlyList<string> roles = await userAdmin.GetUserRolesAsync(user);

        IReadOnlyList<Claim> claims = CreateUserClaims(user, roles);
        
        AccessTokenResult result = jwtTokenService.GenerateJwtAccessToken(claims);
        
        return new AuthResponse
        {
            AccessToken = result.AccessToken,
            AccessTokenExpiry = result.Expiration,
            User = new UserDto(
                UserId: user.Id, 
                Email: user.Email!, 
                FirstName: user.FirstName, 
                LastName: user.LastName,
                Roles: roles)
        };
    }
    
    public async Task<AuthResponse> AuthenticateExternalAsync(ExternalLoginInfo loginInfo)
    {
        AppUser? user = await userAdmin.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

        if (user == null)
        {
            string? email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AuthResponse { FailureReason = "Email not available from external provider" };
            }

            user = new AppUser
            {
                Email = email,
                FirstName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                UserName = email
            };

            IdentityResult? creationResult = await userAdmin.CreateUserAsync(user);
            if (!creationResult.Succeeded)
            {
                return new AuthResponse { FailureReason = "User creation failed" };
            }

            IdentityResult? addLoginResult = await userAdmin.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                return new AuthResponse { FailureReason = "Failed to link external login" };
            }
        }

        IReadOnlyList<string>? roles = await userAdmin.GetUserRolesAsync(user);
        List<Claim>? claims = CreateUserClaims(user, roles);
        AccessTokenResult? tokenResult = jwtTokenService.GenerateJwtAccessToken(claims);

        return new AuthResponse
        {
            AccessToken = tokenResult.AccessToken,
            AccessTokenExpiry = tokenResult.Expiration,
            User = new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles)
        };
    }


    private static List<Claim> CreateUserClaims(AppUser user, IReadOnlyList<string> roles)
    {
        if (user.Email is null)
            throw new ArgumentException(nameof(user.Email));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
        ];
 
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;        
    }
}