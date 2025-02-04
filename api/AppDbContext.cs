using Microsoft.EntityFrameworkCore;
using Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }
    public DbSet<LaboratoryTest> LaboratoryTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>()
            .OwnsOne(p => p.Address);

        modelBuilder.Entity<MedicalHistory>()
            .HasOne(m => m.Patient)
            .WithMany(p => p.MedicalHistory)
            .HasForeignKey(m => m.PatientId);

        modelBuilder.Entity<LaboratoryTest>()
            .HasOne(l => l.Patient)
            .WithMany(p => p.LaboratoryTests)
            .HasForeignKey(l => l.PatientId);
    }
}