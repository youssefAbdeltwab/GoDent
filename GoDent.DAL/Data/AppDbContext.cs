using Microsoft.EntityFrameworkCore;
using GoDent.DAL.Entities;

namespace GoDent.DAL.Data
{
    /// <summary>
    /// The AppDbContext is EF Core's bridge between your C# classes and the SQLite database.
    /// It manages the connection, tracks changes, and translates LINQ to SQL.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Each DbSet maps to a table in the database
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ClinicDebt> ClinicDebts { get; set; }
        public DbSet<ToothHistory> ToothHistories { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed static department data (English names only)
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Surgery", Description = "Dental Surgery Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 2, Name = "Radiology", Description = "Radiology Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 3, Name = "Sterilization", Description = "Sterilization Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 4, Name = "Orthodontics", Description = "Orthodontics Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 5, Name = "Endodontics", Description = "Endodontics Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 6, Name = "Fixed Prothodontics", Description = "Fixed Prothodontics Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 7, Name = "Removable", Description = "Removable Prothodontics Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 8, Name = "Conservative", Description = "Conservative Dentistry Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 9, Name = "Pedodontics", Description = "Pediatric Dentistry Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 10, Name = "Periodontics", Description = "Periodontics Department", CreatedAt = new DateTime(2026, 2, 20) },
                new Department { Id = 11, Name = "Other", Description = "Other Departments", CreatedAt = new DateTime(2026, 2, 20) }
            );
        }
    }
}
