using Game_Vision.Domain;
using Game_Vision.Domain.Basic;
using Game_Vision.Domain.Logs;
using Microsoft.EntityFrameworkCore;

namespace Game_Vision.Models;

public partial class GameVisionDbContext : DbContext
{
    public GameVisionDbContext()
    {
    }

    public GameVisionDbContext(DbContextOptions<GameVisionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Log> Logs { get; set; }
    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }
    //public virtual DbSet<GameGenre> GameGenres { get; set; }
    //public virtual DbSet<GamePlatform> GamePlatforms { get; set; }
   // public virtual DbSet<GameTag> GameTags { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameImage> GameImages { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<PccompatibilityResult> PccompatibilityResults { get; set; }

    public virtual DbSet<Platform> Platforms { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SystemRequirement> SystemRequirements { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }

    public virtual DbSet<UserPcspec> UserPcspecs { get; set; }
    public virtual DbSet<CpuList> CpuList { get; set; }
    public virtual DbSet<GpuList> GpuList { get; set; }
    public virtual DbSet<OsList> OsList { get; set; }
    public virtual DbSet<RamList> RamList { get; set; }
    public DbSet<StorageList> StorageList { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC07053A7D71");

            entity.HasIndex(e => e.NewsId, "IX_Comments_NewsId");

            entity.HasIndex(e => e.ParentId, "IX_Comments_ParentId");

            entity.HasIndex(e => e.ReviewId, "IX_Comments_ReviewId");

            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsApproved).HasDefaultValue(true);

            entity.HasOne(d => d.News).WithMany(p => p.Comments)
                .HasForeignKey(d => d.NewsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_NewsId_News");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Comments_ParentId_Comments");

            entity.HasOne(d => d.Review).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_ReviewId_Reviews");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_UserId_Users");
        });
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.TimeStamp).IsDescending();
            entity.HasIndex(e => new { e.TimeStamp, e.Level }).IsDescending();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ClientIP);
            entity.HasIndex(e => e.RequestPath);
            entity.HasIndex(e => e.StatusCode);
            entity.HasIndex(e => e.ElapsedMs).IsDescending();
        });
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Develope__3214EC07ABE29868");

            entity.HasIndex(e => e.Name, "UQ__Develope__737584F628815AF6").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Develope__BC7B5FB65250B282").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Slug).HasMaxLength(150);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
        });
        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(gg => new { gg.GameId, gg.GenreId });  // این خط کافیه!
                                                                 // بقیه تنظیمات...
        });

        modelBuilder.Entity<GamePlatform>(entity =>
        {
            entity.HasKey(gp => new { gp.GameId, gp.PlatformId });
            // ...
        });

        modelBuilder.Entity<GameTag>(entity =>
        {
            entity.HasKey(gt => new { gt.GameId, gt.TagId });
            // ...
        });
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC076B856721");

            entity.HasIndex(e => e.GameId, "IX_Favorites_GameId");

            entity.HasIndex(e => e.UserId, "IX_Favorites_UserId");

            entity.HasIndex(e => new { e.UserId, e.GameId }, "UQ__Favorite__D5234532DFF7EAA1").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Favorites_GameId_Games");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Favorites_UserId_Users");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Games__3214EC070F921C00");

            entity.HasIndex(e => e.DeveloperId, "IX_Games_DeveloperId");

            entity.HasIndex(e => e.PublisherId, "IX_Games_PublisherId");

            entity.HasIndex(e => e.ReleaseDate, "IX_Games_ReleaseDate").IsDescending();

            entity.HasIndex(e => e.Slug, "IX_Games_Slug");

            entity.HasIndex(e => e.Title, "IX_Games_Title");

            entity.HasIndex(e => e.Slug, "UQ__Games__BC7B5FB65770FA04").IsUnique();

            entity.Property(e => e.AverageRating)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 1)");
            entity.Property(e => e.BannerImageUrl).HasMaxLength(500);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.TotalReviews).HasDefaultValue(0);
            entity.Property(e => e.TrailerUrl).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Developer).WithMany(p => p.Games)
                .HasForeignKey(d => d.DeveloperId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Games_DeveloperId_Developers");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Games)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Games_PublisherId_Publishers");

            entity.HasMany(d => d.Genres).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GameGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK_GameGenres_GenreId_Genres"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .HasConstraintName("FK_GameGenres_GameId_Games"),
                    j =>
                    {
                        j.HasKey("GameId", "GenreId").HasName("PK__GameGenr__DA80C7AAE14CD72C");
                        j.ToTable("GameGenres");
                    });

            entity.HasMany(d => d.Platforms).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GamePlatform",
                    r => r.HasOne<Platform>().WithMany()
                        .HasForeignKey("PlatformId")
                        .HasConstraintName("FK_GamePlatforms_PlatformId_Platforms"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .HasConstraintName("FK_GamePlatforms_GameId_Games"),
                    j =>
                    {
                        j.HasKey("GameId", "PlatformId").HasName("PK__GamePlat__95ED0892A4289821");
                        j.ToTable("GamePlatforms");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GameTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_GameTags_TagId_Tags"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .HasConstraintName("FK_GameTags_GameId_Games"),
                    j =>
                    {
                        j.HasKey("GameId", "TagId").HasName("PK__GameTags__FCEF58676E07CBE0");
                        j.ToTable("GameTags");
                    });
        });

        modelBuilder.Entity<GameImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameImag__3214EC07FAAC4EF5");

            entity.HasIndex(e => e.GameId, "IX_GameImages_GameId");

            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Game).WithMany(p => p.GameImages)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_GameImages_GameId_Games");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC07A459B492");

            entity.HasIndex(e => e.Name, "UQ__Genres__737584F63699BE4E").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Genres__BC7B5FB6196067D4").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC07D2D615D3");

            entity.HasIndex(e => e.AuthorId, "IX_News_AuthorId");

            entity.HasIndex(e => e.PublishedAt, "IX_News_PublishedAt").IsDescending();

            entity.HasIndex(e => e.Slug, "IX_News_Slug");

            entity.HasIndex(e => e.Slug, "UQ__News__BC7B5FB6B0E0108D").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.Slug).HasMaxLength(300);
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.News)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_AuthorId_Users");
        });

        modelBuilder.Entity<PccompatibilityResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PCCompat__3214EC07C9E4697A");

            entity.ToTable("PCCompatibilityResults");

            entity.HasIndex(e => e.CheckedAt, "IX_PCCompatibility_CheckedAt").IsDescending();

            entity.HasIndex(e => e.GameId, "IX_PCCompatibility_GameId");

            entity.HasIndex(e => e.UserId, "IX_PCCompatibility_UserId");

            entity.HasIndex(e => new { e.UserId, e.GameId }, "UQ__PCCompat__D5234532A1C94B88").IsUnique();

            entity.Property(e => e.Bottleneck).HasMaxLength(500);
            entity.Property(e => e.CheckedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Score).HasColumnType("decimal(4, 1)");

            entity.HasOne(d => d.Game).WithMany(p => p.PccompatibilityResults)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_PCCompatibility_GameId_Games");

            entity.HasOne(d => d.User).WithMany(p => p.PccompatibilityResults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PCCompatibility_UserId_Users");
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Platform__3214EC074E72B1DB");

            entity.HasIndex(e => e.Name, "UQ__Platform__737584F63249BF9F").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Platform__BC7B5FB66F2D066E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Publishe__3214EC072619FB3A");

            entity.HasIndex(e => e.Name, "UQ__Publishe__737584F6780369B3").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Publishe__BC7B5FB622F29145").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Slug).HasMaxLength(150);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC076FBCB718");

            entity.HasIndex(e => e.GameId, "IX_Reviews_GameId");

            entity.HasIndex(e => e.Rating, "IX_Reviews_Rating");

            entity.HasIndex(e => e.UserId, "IX_Reviews_UserId");

            entity.HasIndex(e => new { e.GameId, e.UserId }, "UQ__Reviews__FBC01B386F6FE977").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Reviews_GameId_Games");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Reviews_UserId_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07828C4A4B");

            entity.HasIndex(e => e.Name, "IX_Roles_Name");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F64E0B1205").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<SystemRequirement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemRe__3214EC07897BA9AB");

            entity.HasIndex(e => e.GameId, "IX_SystemRequirements_GameId");

            entity.HasIndex(e => e.GameId, "UQ__SystemRe__2AB897FC473FB7CC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MinCpu)
                .HasMaxLength(200)
                .HasColumnName("MinCPU");
            entity.Property(e => e.MinDirectX).HasMaxLength(50);
            entity.Property(e => e.MinGpu)
                .HasMaxLength(200)
                .HasColumnName("MinGPU");
            entity.Property(e => e.MinOs)
                .HasMaxLength(200)
                .HasColumnName("MinOS");
            entity.Property(e => e.MinRam).HasColumnName("MinRAM");
            entity.Property(e => e.RecCpu)
                .HasMaxLength(200)
                .HasColumnName("RecCPU");
            entity.Property(e => e.RecDirectX).HasMaxLength(50);
            entity.Property(e => e.RecGpu)
                .HasMaxLength(200)
                .HasColumnName("RecGPU");
            entity.Property(e => e.RecOs)
                .HasMaxLength(200)
                .HasColumnName("RecOS");
            entity.Property(e => e.RecRam).HasColumnName("RecRAM");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithOne(p => p.SystemRequirement)
                .HasForeignKey<SystemRequirement>(d => d.GameId)
                .HasConstraintName("FK_SystemRequirements_GameId_Games");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07773078B3");

            entity.HasIndex(e => e.Name, "UQ__Tags__737584F613DE3254").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Tags__BC7B5FB6CA37CE20").IsUnique();

            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .HasDefaultValue("#007BFF");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07A9A742C9");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4A903A76D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D4980DC3").IsUnique();

            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_RoleId_Roles");
        });

        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FollowedId }).HasName("PK__UserFoll__F7A5FC9F24953289");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Followed).WithMany(p => p.UserFollowFolloweds)
                .HasForeignKey(d => d.FollowedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFollows_FollowedId_Users");

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFollows_FollowerId_Users");
        });

        modelBuilder.Entity<UserPcspec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPCSp__3214EC074D4A64ED");

            entity.ToTable("UserPCSpecs");

            entity.HasIndex(e => e.UserId, "IX_UserPCSpecs_UserId");

            entity.HasIndex(e => e.UserId, "UQ__UserPCSp__1788CC4DD78110BD").IsUnique();

            entity.Property(e => e.Cpu)
                .HasMaxLength(200)
                .HasColumnName("CPU");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DirectX).HasMaxLength(50);
            entity.Property(e => e.Gpu)
                .HasMaxLength(200)
                .HasColumnName("GPU");
            entity.Property(e => e.Os)
                .HasMaxLength(200)
                .HasColumnName("OS");
            entity.Property(e => e.Ram).HasColumnName("RAM");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithOne(p => p.UserPcspec)
                .HasForeignKey<UserPcspec>(d => d.UserId)
                .HasConstraintName("FK_UserPCSpecs_UserId_Users");
        });
        

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
