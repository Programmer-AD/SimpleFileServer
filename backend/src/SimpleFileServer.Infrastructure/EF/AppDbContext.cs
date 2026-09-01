using Microsoft.EntityFrameworkCore;
using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Infrastructure.EF;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<DomainFile> DomainFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
