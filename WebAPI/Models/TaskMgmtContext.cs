using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public partial class TaskMgmtContext : DbContext
{
    public TaskMgmtContext()
    {
    }

    public TaskMgmtContext(DbContextOptions<TaskMgmtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TaskSkill> TaskSkills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSkill> UserSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=.\\MSSQLSERVER3;Database=TaskMgmt;User=sa;Password=SQL;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UQ__Managers__1788CC4D816A5785").IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.UserId)
                .HasConstraintName("FK_Managers_UserId");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Manager).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Tasks_ManagerId");
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.Property(e => e.AssignedAt)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskAssignments_TaskId");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskAssignments_UserId");
        });

        modelBuilder.Entity<TaskSkill>(entity =>
        {
            entity.HasOne(d => d.Skill).WithMany(p => p.TaskSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSkills_SkillId");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskSkills)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSkills_TaskId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(256);
            entity.Property(e => e.LastName).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(256);
            entity.Property(e => e.PwdHash).HasMaxLength(256);
            entity.Property(e => e.PwdSalt).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.HasOne(d => d.Skill).WithMany(p => p.UserSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSkills_SkillId");

            entity.HasOne(d => d.User).WithMany(p => p.UserSkills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSkills_UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
