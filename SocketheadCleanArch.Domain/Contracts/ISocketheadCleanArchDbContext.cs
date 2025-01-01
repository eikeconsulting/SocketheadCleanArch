using Microsoft.EntityFrameworkCore;
using SocketheadCleanArch.Domain.Data.Entities;

namespace SocketheadCleanArch.Domain.Contracts;

public interface ISocketheadCleanArchDbContext
{
    DbSet<EFMigrationsHistory> EFMigrationsHistory { get; }

}