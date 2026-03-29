using Microsoft.EntityFrameworkCore;
using DepartmentService.Models;

namespace DepartmentService.Data
{
    public class DepartmentDbContext : DbContext
    {
        public DepartmentDbContext(DbContextOptions<DepartmentDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.DepartmentId);
                
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(d => d.Location)
                    .HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
