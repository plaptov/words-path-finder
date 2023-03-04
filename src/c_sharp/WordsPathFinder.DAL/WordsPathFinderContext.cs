using Microsoft.EntityFrameworkCore;

namespace WordsPathFinder.DAL;

public class WordsPathFinderContext : DbContext
{
    public DbSet<WordsPathModel> WordsPaths { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql("Host=localhost;Database=WordsPaths;Username=test;Password=123456");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WordsPathModel>()
           .HasKey(m => new
           {
               m.From,
               m.To,
           });
    }
}
