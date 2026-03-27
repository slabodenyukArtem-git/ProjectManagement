using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectManagement.DAL.Entities;
using TaskEntity = ProjectManagement.DAL.Entities.Task;

namespace ProjectManagement.DAL;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Executors)
            .WithMany(e => e.ParticipatingProjects)
            .UsingEntity(j => j.ToTable("ProjectExecutors"));

        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectManager)
            .WithMany(e => e.LeadingProjects)
            .HasForeignKey(p => p.ProjectManagerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.StartDate);
        
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Priority);
        
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();
        
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(t => t.Comment)
                .HasMaxLength(1000);
            
            entity.Property(t => t.Priority)
                .IsRequired();
            
            entity.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(t => t.Executor)
                .WithMany()
                .HasForeignKey(t => t.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.Priority);
            entity.HasIndex(t => t.ProjectId);
        });
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
