using MedicalDashboardAPI.Models;
using MedicalDashboardAPI;
using Microsoft.EntityFrameworkCore;

namespace MedicalDashboardAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<MedicalFile> MedicalFiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.MedicalFiles)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);
        }
    }
}