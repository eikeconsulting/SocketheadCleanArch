using Microsoft.EntityFrameworkCore;
using SocketheadCleanArch.Domain.Contracts;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.Service.Repository;

public class SocketheadCleanArchRepository(ISocketheadCleanArchDbContext context)
{
    public async Task<IReadOnlyList<EFMigrationsHistory>> GetMigrationsHistoryAsync() => 
        await context
            .EFMigrationsHistory
            .AsNoTracking()
            .ToListAsync();
    
}