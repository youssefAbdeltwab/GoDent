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
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ToothHistory> ToothHistories { get; set; }

    }
}
