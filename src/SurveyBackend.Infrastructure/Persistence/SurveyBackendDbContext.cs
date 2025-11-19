using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Infrastructure.Persistence;

public class SurveyBackendDbContext : DbContext
{
    public SurveyBackendDbContext(DbContextOptions<SurveyBackendDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(u => u.DepartmentId)
                .IsRequired();
            entity.Property(u => u.CreatedAt)
                .IsRequired();
            entity.Property(u => u.UpdatedAt)
                .IsRequired();
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("UserRefreshTokens");
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(512);
            entity.Property(rt => rt.CreatedAt)
                .IsRequired();
            entity.Property(rt => rt.ExpiresAt)
                .IsRequired();
            entity.Property(rt => rt.UserId)
                .IsRequired();
            entity.HasIndex(rt => rt.Token)
                .IsUnique();
        });
    }
}
