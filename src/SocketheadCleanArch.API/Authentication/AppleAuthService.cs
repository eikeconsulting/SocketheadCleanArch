using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SocketheadCleanArch.API.Utils;

namespace SocketheadCleanArch.API.Authentication;

public class AppleAuthService(
    IHttpClientFactory httpClientFactory,
    IConfiguration config,
    ILogger<AppleAuthService> logger)
{
    public async Task<bool> ValidateTokenAsync(string idToken)
    {
        try
        {
            string? keysUrl = config["Authentication:Apple:TokenKeysEndpoint"];

            HttpClient client = httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(keysUrl);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(content);
            JsonElement keys = json.RootElement.GetProperty("keys");

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = config["Authentication:Apple:Issuer"],
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys.EnumerateArray()
                    .Select(AuthHelper.CreateSecurityKey)
            };

            JwtSecurityTokenHandler handler = new();
            handler.ValidateToken(idToken, validationParameters, out _);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Apple token validation failed");
            return false;
        }
    }
}