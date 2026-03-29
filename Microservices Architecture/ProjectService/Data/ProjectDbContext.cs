using Microsoft.EntityFrameworkCore;
using ProjectService.Models;

namespace ProjectService.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeProject> EmployeeProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.ProjectId);
                
                entity.Property(p => p.ProjectName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(p => p.Description)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<EmployeeProject>(entity =>
            {
                entity.HasKey(ep => ep.EmployeeProjectId);
                
                entity.Property(ep => ep.Role)
                    .HasMaxLength(50);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
