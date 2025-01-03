using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocketheadCleanArch.Domain.Contracts;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Admin.Data;

public class SocketheadCleanArchDbContext(DbContextOptions<SocketheadCleanArchDbContext> options) : 
    IdentityDbContext<AppUser, AppRole, string>(options), 
    ISocketheadCleanArchDbContext
{
    public DbSet<EFMigrationsHistory> EFMigrationsHistory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<EFMigrationsHistory>()
            .ToTable("__EFMigrationsHistory")
            .HasKey(m => m.MigrationId);
        
        base.OnModelCreating(builder);
    }
}
