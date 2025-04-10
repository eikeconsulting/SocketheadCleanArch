using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.API.Utils;

public static class AuthHelper
{
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

    public static SecurityKey CreateSecurityKey(JsonElement key)
    {
        string n = key.GetProperty("n").GetString()!;
        string e = key.GetProperty("e").GetString()!;

        RSAParameters rsaParameters = new RSAParameters
        {
            Modulus = Base64UrlEncoder.DecodeBytes(n),
            Exponent = Base64UrlEncoder.DecodeBytes(e)
        };

        RSA rsa = RSA.Create();
        rsa.ImportParameters(rsaParameters);

        return new RsaSecurityKey(rsa)
        {
            KeyId = key.GetProperty("kid").GetString()
        };
    }
}