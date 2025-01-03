namespace SocketheadCleanArch.API.Authentication;

public class JwtTokenSettings
{
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int AccessTokenExpirationMinutes { get; set; }
    public required int RefreshTokenExpirationDays { get; set; }
}
