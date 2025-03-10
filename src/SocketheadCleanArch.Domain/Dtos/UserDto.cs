namespace SocketheadCleanArch.Domain.Dtos;

public record UserDto(string UserId, string Email, string? FirstName, string? LastName, IReadOnlyList<string> Roles);
