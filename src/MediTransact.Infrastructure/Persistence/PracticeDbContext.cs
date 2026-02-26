using MediTransact.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediTransact.Infrastructure.Persistence;

public class PracticeDbContext(DbContextOptions<PracticeDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InsuranceCarrier> InsuranceCarriers => Set<InsuranceCarrier>();
    public DbSet<PatientInsurancePlan> PatientInsurancePlans => Set<PatientInsurancePlan>();
    public DbSet<SupportedInsurancePlan> SupportedInsurancePlans => Set<SupportedInsurancePlan>();
    public DbSet<PatientCase> PatientCases => Set<PatientCase>();
    public DbSet<CaseCharge> CaseCharges => Set<CaseCharge>();
    public DbSet<CaseChargeCode> CaseChargeCodes => Set<CaseChargeCode>();
    public DbSet<ChargeCodeType> ChargeCodeTypes => Set<ChargeCodeType>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<PracticeLocation> PracticeLocations => Set<PracticeLocation>();
    public DbSet<ProviderScheduleTemplate> ProviderScheduleTemplates => Set<ProviderScheduleTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>().HasIndex(x => x.Npi).IsUnique();
        modelBuilder.Entity<Invoice>().Property(x => x.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>().Property(x => x.PaidAmount).HasPrecision(18, 2);


        modelBuilder.Entity<Patient>()
            .HasMany(x => x.Addresses)
            .WithOne()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tenant>()
            .HasMany(x => x.Addresses)
            .WithOne()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PracticeLocation>()
            .HasMany(x => x.Addresses)
            .WithOne()
            .HasForeignKey(x => x.PracticeLocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InsuranceCarrier>()
            .HasMany(x => x.Addresses)
            .WithOne()
            .HasForeignKey(x => x.InsuranceCarrierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(x => x.InsurancePlans)
            .WithOne()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(x => x.Cases)
            .WithOne()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne<PracticeLocation>()
            .WithMany()
            .HasForeignKey(x => x.PracticeLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProviderScheduleTemplate>()
            .HasOne<Provider>()
            .WithMany()
            .HasForeignKey(x => x.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProviderScheduleTemplate>()
            .HasOne<PracticeLocation>()
            .WithMany()
            .HasForeignKey(x => x.PracticeLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PracticeLocation>()
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceCarrier>()
            .HasMany(x => x.SupportedPlans)
            .WithOne()
            .HasForeignKey(x => x.InsuranceCarrierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientCase>()
            .HasMany(x => x.Charges)
            .WithOne()
            .HasForeignKey(x => x.PatientCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CaseCharge>()
            .HasMany(x => x.ChargeCodes)
            .WithOne()
            .HasForeignKey(x => x.CaseChargeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientInsurancePlan>()
            .HasIndex(x => new { x.PatientId, x.InsuranceCarrierId, x.MemberNumber });

        modelBuilder.Entity<SupportedInsurancePlan>()
            .HasIndex(x => new { x.InsuranceCarrierId, x.PlanCode }).IsUnique();

        modelBuilder.Entity<CaseChargeCode>()
            .HasIndex(x => new { x.CaseChargeId, x.ChargeCodeTypeId, x.Code });

        modelBuilder.Entity<ChargeCodeType>()
            .HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<Tenant>()
            .HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<PracticeLocation>()
            .HasIndex(x => new { x.TenantId, x.LocationName }).IsUnique();

        modelBuilder.Entity<ProviderScheduleTemplate>()
            .HasIndex(x => new { x.ProviderId, x.PracticeLocationId, x.DayOfWeek, x.StartTime, x.EndTime });

        modelBuilder.Entity<Address>()
            .HasIndex(x => new { x.AddressType, x.PhoneType, x.PostalCode });
    }
}
