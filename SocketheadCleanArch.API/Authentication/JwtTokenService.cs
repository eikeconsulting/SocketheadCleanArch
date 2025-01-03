using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SocketheadCleanArch.API.Authentication;

public record AccessTokenResult(string AccessToken, DateTime Expiration);

public class JwtTokenService(IOptions<JwtTokenSettings> jwtTokenSettings)
{
    private JwtTokenSettings Settings => jwtTokenSettings.Value;
    
    /// <summary>
    /// Generates Access Token
    /// </summary>
    public AccessTokenResult GenerateJwtAccessToken(IEnumerable<Claim> claims)
    {
        var expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(Settings.AccessTokenExpirationMinutes));
        
        return new AccessTokenResult(
            AccessToken: new JwtSecurityTokenHandler()
                .WriteToken(new JwtSecurityToken(
                    issuer: Settings.Issuer,
                    audience: Settings.Audience,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    claims: claims,
                    signingCredentials: new SigningCredentials(
                        key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.Secret)),
                        algorithm: SecurityAlgorithms.HmacSha256Signature))),
            Expiration: expires);
    }
}