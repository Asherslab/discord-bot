using Microsoft.EntityFrameworkCore;

namespace MineColonies.Discord.Assistant.Module.Leveling.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<LevelingEntry> LevelingEntries { get; set; } = null!;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LevelingEntry>()
                .HasKey(entry => entry.DiscordId);
        }
    }
}