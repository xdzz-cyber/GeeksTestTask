using GeeksTestTask.DLL.EntityTypeConfigurations;
using GeeksTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace GeeksTestTask.DLL;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FolderConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    }


    public DbSet<Folder> Folders { get; set; }
}