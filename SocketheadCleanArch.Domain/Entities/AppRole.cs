using Microsoft.AspNetCore.Identity;

namespace SocketheadCleanArch.Domain.Entities;

public class AppRole : IdentityRole
{
    public AppRole() : base() { }
    public AppRole(string roleName) : base(roleName) { }
}