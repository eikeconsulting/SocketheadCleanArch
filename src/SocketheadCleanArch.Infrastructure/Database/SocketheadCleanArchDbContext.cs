using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
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
        
        // This is required for Identity to work properly due to changes in EF Core 3.x
        // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.x/breaking-changes#string-and-byte-array-keys-are-not-client-generated-by-default
        builder
            .Entity<AppUser>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        
        base.OnModelCreating(builder);
    }
}
