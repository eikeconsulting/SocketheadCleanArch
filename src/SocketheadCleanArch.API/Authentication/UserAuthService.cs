using System.Security.Claims;
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