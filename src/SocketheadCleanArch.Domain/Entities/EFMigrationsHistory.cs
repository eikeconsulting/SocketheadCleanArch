namespace SocketheadCleanArch.Domain.Entities;

public class EFMigrationsHistory
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string MigrationId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string ProductVersion { get; set; }
}
