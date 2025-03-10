namespace SocketheadCleanArch.Admin.Models;

public class EditUserRolesVM
{
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public IReadOnlyList<string> AssignedRoles { get; set; } = new List<string>();
    public IReadOnlyList<string?> AvailableRoles { get; set; } = new List<string>();
    public IReadOnlyList<string> SelectedRoles { get; set; } = new List<string>();
}