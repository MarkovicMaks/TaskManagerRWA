using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

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
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:TaskM");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Managers__3214EC07E0AEF2EC");

            entity.HasIndex(e => e.UserId, "UQ__Managers__1788CC4D5C522F07").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.UserId)
                .HasConstraintName("FK__Managers__UserId__52593CB8");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Skills__3214EC078BF45E9F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC07C3140ED9");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Manager).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Tasks__ManagerId__534D60F1");
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskAssi__3214EC0763F8509F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssignedAt)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskAssig__TaskI__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskAssig__UserI__4F7CD00D");
        });

        modelBuilder.Entity<TaskSkill>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Skill).WithMany()
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK__TaskSkill__Skill__4D94879B");

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskSkill__TaskI__4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07DF61039B");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
            entity.HasNoKey();

            entity.HasOne(d => d.Skill).WithMany()
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK__UserSkill__Skill__5165187F");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserSkill__UserI__5070F446");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
