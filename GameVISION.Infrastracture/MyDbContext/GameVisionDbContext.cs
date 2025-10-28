using GameVISION.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameVISION.Infrastracture.MyDbContext
{
    public class GameVisionDbContext : DbContext
    {
        public GameVisionDbContext(DbContextOptions<GameVisionDbContext> options)
            : base(options)
        {
        }

        // 🧍‍♂️ کاربران
        public DbSet<User> Users { get; set; }
        // 🎮 بازی‌ها
        public DbSet<Game> Games { get; set; }
        // 🗞️ پست‌ها
        public DbSet<Post> Posts { get; set; }
        // 💬 نظرات
        public DbSet<Comment> Comments { get; set; }
        // ❤️ لایک‌ها
        public DbSet<Like> Likes { get; set; }
        // 👥 دنبال‌کردن‌ها
        public DbSet<Follow> Follows { get; set; }
        // ⭐ امتیازدهی بازی‌ها
        public DbSet<GameRating> GameRatings { get; set; }
        // 🔖 تگ‌ها
        public DbSet<GameTag> GameTags { get; set; }
        // 🔗 ارتباط بین تگ و بازی
        public DbSet<GameTagRelation> GameTagRelations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تنظیم کلید مرکب برای GameTagRelation
            modelBuilder.Entity<GameTagRelation>()
                .HasKey(gt => new { gt.GameId, gt.TagId });

            // رابطه بازی ↔ تگ
            modelBuilder.Entity<GameTagRelation>()
                .HasOne(gt => gt.Game)
                .WithMany(g => g.GameTags)
                .HasForeignKey(gt => gt.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameTagRelation>()
                .HasOne(gt => gt.Tag)
                .WithMany(t => t.GameTagRelations)
                .HasForeignKey(gt => gt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه دنبال‌کردن (کاربر ↔ کاربر)
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.NoAction);

            // جلوگیری از تکرار Like برای هر کاربر روی هر پست
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique();

            // جلوگیری از تکرار Follow
            modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();

            // جلوگیری از تکرار Rating یک کاربر برای یک بازی
            modelBuilder.Entity<GameRating>()
                .HasIndex(r => new { r.UserId, r.GameId })
                .IsUnique();
        }
    }
}
