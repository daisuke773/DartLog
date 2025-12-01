using DartLog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DartLog.Data
{
    // ★ DbContext → IdentityDbContext<ApplicationUser> に変更
    public class DartLogContext : IdentityDbContext<ApplicationUser>
    {
        public DartLogContext(DbContextOptions<DartLogContext> options)
            : base(options)
        {
        }

        // 「プレイヤー＝ログインユーザー」にするので、Player は段階的に廃止方向
        // いったん残したいならコメントアウトを外す
        // public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Throw> Throws { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ★ Identity のテーブル定義を作るために必ず呼ぶ
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Throw>()
                .HasIndex(t => new { t.GameId, t.RoundNo, t.DartNo })
                .IsUnique();

            // 必要ならここに Game と User のリレーションをさらに明示してもOK
            // modelBuilder.Entity<Game>()
            //     .HasOne(g => g.User)
            //     .WithMany(u => u.Games)
            //     .HasForeignKey(g => g.UserId);
        }
    }
}
