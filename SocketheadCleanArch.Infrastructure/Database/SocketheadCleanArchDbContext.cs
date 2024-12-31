using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SocketheadCleanArch.Admin.Data;

public class SocketheadCleanArchDbContext(DbContextOptions<SocketheadCleanArchDbContext> options) : IdentityDbContext(options);
