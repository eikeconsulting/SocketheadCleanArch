namespace SocketheadCleanArch.Admin.Seeding;

public record UserSeed(string Email, string Password, string[] Roles);

public class DataSeedSettings
{
    public required string[] Roles { get; set; }
    public required UserSeed[] Users { get; set; }
}