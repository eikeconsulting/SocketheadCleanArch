using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.API.Utils;

public static class AuthHelper
{
    public static async Task<bool> ValidateAppleTokenSignatureAsync(string idToken)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync("https://appleid.apple.com/auth/keys");
        string content = await response.Content.ReadAsStringAsync();

        JsonDocument appleKeys = JsonDocument.Parse(content);
        JsonElement keys = appleKeys.RootElement.GetProperty("keys");

        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = "https://appleid.apple.com",
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys.EnumerateArray().Select(AppleKeyHelper.CreateSecurityKey)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        try
        {
            tokenHandler.ValidateToken(idToken, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static List<Claim> CreateUserClaims(AppUser user, IReadOnlyList<string> roles)
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