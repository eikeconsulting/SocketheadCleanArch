using Microsoft.AspNetCore.Identity;

namespace SocketheadCleanArch.Domain.Data.Entities;

public class AppRole : IdentityRole
{
    public AppRole() : base() { }
    public AppRole(string roleName) : base(roleName) { }
}