using Microsoft.EntityFrameworkCore;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Domain.Contracts;

public interface ISocketheadCleanArchDbContext
{
    DbSet<EFMigrationsHistory> EFMigrationsHistory { get; }

}