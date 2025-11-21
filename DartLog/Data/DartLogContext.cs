using Microsoft.EntityFrameworkCore;
using DartLog.Models;

namespace DartLog.Data
{
    public class DartLogContext : DbContext
    {
        public DartLogContext(DbContextOptions<DartLogContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Throw> Throws { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // throws のユニーク制約 (game_id, round_no, dart_no)
            modelBuilder.Entity<Throw>()
                .HasIndex(t => new { t.GameId, t.RoundNo, t.DartNo })
                .IsUnique();
        }
    }
}
