using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Roles;
using SurveyBackend.Domain.Surveys;
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
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<DependentQuestion> DependentQuestions => Set<DependentQuestion>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

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

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("Surveys");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(s => s.Description)
                .HasMaxLength(2000);
            entity.Property(s => s.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(s => s.DepartmentId)
                .IsRequired();
            entity.Property(s => s.CreatedAt)
                .IsRequired();
            entity.Property(s => s.AccessType)
                .IsRequired();
            entity.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(false);
            entity.Property(s => s.StartDate);
            entity.Property(s => s.EndDate);

            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Questions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(1000);
            entity.Property(q => q.Description)
                .HasMaxLength(500);
            entity.Property(q => q.Order)
                .IsRequired();
            entity.Property(q => q.Type)
                .IsRequired();
            entity.Property(q => q.IsRequired)
                .IsRequired();

            entity.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<Answer>()
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.ToTable("QuestionOptions");
            entity.HasKey(qo => qo.Id);
            entity.Property(qo => qo.Text)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(qo => qo.Order)
                .IsRequired();
            entity.Property(qo => qo.Value);

            entity.HasMany(qo => qo.DependentQuestions)
                .WithOne(dq => dq.ParentOption)
                .HasForeignKey(dq => dq.ParentQuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DependentQuestion>(entity =>
        {
            entity.ToTable("DependentQuestions");
            entity.HasKey(dq => dq.Id);
            entity.Property(dq => dq.ParentQuestionOptionId)
                .IsRequired();
            entity.Property(dq => dq.ChildQuestionId)
                .IsRequired();

            entity.HasOne(dq => dq.ChildQuestion)
                .WithMany()
                .HasForeignKey(dq => dq.ChildQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.ToTable("Participants");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.ExternalId);
            entity.Property(p => p.LdapUsername)
                .HasMaxLength(100);
            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.HasIndex(p => p.ExternalId)
                .IsUnique()
                .HasFilter("[ExternalId] IS NOT NULL");

            entity.HasIndex(p => p.LdapUsername)
                .IsUnique()
                .HasFilter("[LdapUsername] IS NOT NULL");
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.ToTable("Participations");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.SurveyId)
                .IsRequired();
            entity.Property(p => p.ParticipantId)
                .IsRequired();
            entity.Property(p => p.StartedAt)
                .IsRequired();
            entity.Property(p => p.CompletedAt);
            entity.Property(p => p.IpAddress)
                .HasMaxLength(50);

            entity.HasOne(p => p.Participant)
                .WithMany()
                .HasForeignKey(p => p.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(p => p.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Answers)
                .WithOne(a => a.Participation)
                .HasForeignKey(a => a.ParticipationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answers");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.ParticipationId)
                .IsRequired();
            entity.Property(a => a.QuestionId)
                .IsRequired();
            entity.Property(a => a.TextValue);

            entity.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.SelectedOptions)
                .WithOne(ao => ao.Answer)
                .HasForeignKey(ao => ao.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.ToTable("AnswerOptions");
            entity.HasKey(ao => ao.Id);
            entity.Property(ao => ao.AnswerId)
                .IsRequired();
            entity.Property(ao => ao.QuestionOptionId)
                .IsRequired();

            entity.HasOne(ao => ao.QuestionOption)
                .WithMany()
                .HasForeignKey(ao => ao.QuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
