using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace SocketheadCleanArch.API.Utils;

public static class AppleKeyHelper
{
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