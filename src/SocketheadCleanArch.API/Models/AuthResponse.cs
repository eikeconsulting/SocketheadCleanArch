using SocketheadCleanArch.Domain.Dtos;

namespace SocketheadCleanArch.API.Models;

public class AuthResponse
{
    public bool IsSuccess => !string.IsNullOrEmpty(AccessToken);
    
    public string? FailureReason { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiry { get; init; } = DateTime.UtcNow;
 
    // TODO: support Refresh Token
    //public required string RefreshToken { get; init; }
    //public required DateTime RefreshTokenExpiry { get; init; }
    
    public UserDto? User { get; init; }
}