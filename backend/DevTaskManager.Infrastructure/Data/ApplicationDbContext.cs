using DevTaskManager.Domain.Entities;
using DevTaskManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTaskManager.Domain.Enums.TaskStatus;

namespace DevTaskManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<ProjectType> ProjectTypes => Set<ProjectType>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<DeveloperTechnology> DeveloperTechnologies => Set<DeveloperTechnology>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectDeveloper> ProjectDevelopers => Set<ProjectDeveloper>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ──────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            e.Property(x => x.Role).HasColumnName("role")
                .HasConversion(v => v.ToString().ToLower(),
                               v => Enum.Parse<UserRole>(v, true));
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ── ProjectType ───────────────────────────────────────────────────
        modelBuilder.Entity<ProjectType>(e =>
        {
            e.ToTable("project_types");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
            e.Property(x => x.IsActive).HasColumnName("is_active");
        });

        // ── Developer ─────────────────────────────────────────────────────
        modelBuilder.Entity<Developer>(e =>
        {
            e.ToTable("developers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
            e.Property(x => x.Cedula).HasColumnName("cedula").HasMaxLength(13).IsRequired();
            e.Property(x => x.ProjectTypeId).HasColumnName("project_type_id");
            e.Property(x => x.SeniorityLevel).HasColumnName("seniority_level")
                .HasConversion(v => v.ToString().ToLower(),
                               v => Enum.Parse<SeniorityLevel>(v, true));
            e.Property(x => x.AvailabilityStatus).HasColumnName("availability_status")
                .HasConversion(v => v.ToString().ToLower(),
                               v => Enum.Parse<AvailabilityStatus>(v, true));
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(x => x.User).WithMany(u => u.Developers)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.ProjectType).WithMany()
                .HasForeignKey(x => x.ProjectTypeId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Project ───────────────────────────────────────────────────────
        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("projects");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ProjectCode).HasColumnName("project_code").HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.ProjectTypeId).HasColumnName("project_type_id");
            e.Property(x => x.TechnologyId).HasColumnName("technology_id");
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(v => v == ProjectStatus.InProgress ? "in_progress"
                                  : v == ProjectStatus.OnHold ? "on_hold"
                                  : v.ToString().ToLower(),
                               v => v == "in_progress" ? ProjectStatus.InProgress
                                  : v == "on_hold" ? ProjectStatus.OnHold
                                  : Enum.Parse<ProjectStatus>(v, true));
            e.Property(x => x.StartDate).HasColumnName("start_date");
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.EstimatedHours).HasColumnName("estimated_hours").HasPrecision(8, 2);
            e.Property(x => x.CreatedBy).HasColumnName("created_by");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(x => x.ProjectType).WithMany(pt => pt.Projects)
                .HasForeignKey(x => x.ProjectTypeId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Technology).WithMany()
                .HasForeignKey(x => x.TechnologyId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Creator).WithMany(u => u.CreatedProjects)
                .HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Technology ───────────────────────────────────────────────────
        modelBuilder.Entity<Technology>(e =>
        {
            e.ToTable("technologies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(x => x.ProjectTypeId).HasColumnName("project_type_id");
            e.Property(x => x.IsActive).HasColumnName("is_active");

            e.HasOne(x => x.ProjectType).WithMany()
                .HasForeignKey(x => x.ProjectTypeId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── DeveloperTechnology ──────────────────────────────────────────
        modelBuilder.Entity<DeveloperTechnology>(e =>
        {
            e.ToTable("developer_technologies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.DeveloperId).HasColumnName("developer_id");
            e.Property(x => x.TechnologyId).HasColumnName("technology_id");

            e.HasIndex(x => new { x.DeveloperId, x.TechnologyId }).IsUnique();

            e.HasOne(x => x.Developer).WithMany(d => d.DeveloperTechnologies)
                .HasForeignKey(x => x.DeveloperId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Technology).WithMany()
                .HasForeignKey(x => x.TechnologyId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProjectDeveloper ──────────────────────────────────────────────
        modelBuilder.Entity<ProjectDeveloper>(e =>
        {
            e.ToTable("project_developers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ProjectId).HasColumnName("project_id");
            e.Property(x => x.DeveloperId).HasColumnName("developer_id");
            e.Property(x => x.RoleInProject).HasColumnName("role_in_project").HasMaxLength(100);
            e.Property(x => x.AssignedAt).HasColumnName("assigned_at");

            e.HasIndex(x => new { x.ProjectId, x.DeveloperId }).IsUnique();

            e.HasOne(x => x.Project).WithMany(p => p.ProjectDevelopers)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Developer).WithMany(d => d.ProjectDevelopers)
                .HasForeignKey(x => x.DeveloperId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── TaskItem ──────────────────────────────────────────────────────
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("tasks");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.TaskCode).HasColumnName("task_code").HasMaxLength(20).IsRequired();
            e.Property(x => x.ProjectId).HasColumnName("project_id");
            e.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.AssignedTo).HasColumnName("assigned_to");
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(v => v == TaskStatus.InProgress ? "in_progress"
                                  : v.ToString().ToLower(),
                               v => v == "in_progress" ? TaskStatus.InProgress
                                  : Enum.Parse<TaskStatus>(v, true));
            e.Property(x => x.Priority).HasColumnName("priority")
                .HasConversion(v => v.ToString().ToLower(),
                               v => Enum.Parse<TaskPriority>(v, true));
            e.Property(x => x.EstimatedHours).HasColumnName("estimated_hours").HasPrecision(6, 2);
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.CompletedAt).HasColumnName("completed_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(x => x.Project).WithMany(p => p.Tasks)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.AssignedDeveloper).WithMany(d => d.AssignedTasks)
                .HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Creator).WithMany()
                .HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // ── TaskComment ───────────────────────────────────────────────────
        modelBuilder.Entity<TaskComment>(e =>
        {
            e.ToTable("task_comments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.TaskId).HasColumnName("task_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Comment).HasColumnName("comment").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.Task).WithMany(t => t.Comments)
                .HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.User).WithMany(u => u.Comments)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
