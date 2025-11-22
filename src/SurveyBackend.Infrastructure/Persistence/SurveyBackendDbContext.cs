using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Roles;
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
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(d => d.ExternalIdentifier)
                .IsRequired()
                .HasMaxLength(256);
            entity.HasIndex(d => d.ExternalIdentifier)
                .IsUnique();
        });

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
            entity.Property(u => u.PasswordHash)
                .HasMaxLength(512);
            entity.Property(u => u.IsSuperAdmin)
                .IsRequired();
            entity.Property(u => u.CreatedAt)
                .IsRequired();
            entity.Property(u => u.UpdatedAt)
                .IsRequired();
            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId);
            entity.HasMany(u => u.Roles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(r => r.Description)
                .HasMaxLength(500);
            entity.HasMany(r => r.Permissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.Description)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            entity.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId, ur.DepartmentId });
            entity.Property(ur => ur.DepartmentId)
                .IsRequired();
            entity.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
            entity.HasOne(ur => ur.Department)
                .WithMany()
                .HasForeignKey(ur => ur.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
