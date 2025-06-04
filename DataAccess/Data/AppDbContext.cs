using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Employee entity
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.NationalId)
                .IsUnique(); // Ensure National ID is unique

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique(); // Ensure Email is unique

            // Configure Attendance entity
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId);

            // Ensure one check-in per employee per day
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.EmployeeId, a.CheckInTime })
                .IsUnique();
        }
    }
}
