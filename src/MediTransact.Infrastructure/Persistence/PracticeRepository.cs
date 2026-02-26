using MediTransact.Application.DTOs;
using MediTransact.Application.Interfaces;
using MediTransact.Domain.Entities;
using MediTransact.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediTransact.Infrastructure.Persistence;

public class PracticeRepository(IDbContextFactory<PracticeDbContext> dbContextFactory)
    : IPracticeRepository, IPracticeReadModel, IAsyncDisposable
{
    private readonly PracticeDbContext _db = dbContextFactory.CreateDbContext();

    public Task AddPatientAsync(Patient patient, CancellationToken ct) => _db.Patients.AddAsync(patient, ct).AsTask();
    public Task AddProviderAsync(Provider provider, CancellationToken ct) => _db.Providers.AddAsync(provider, ct).AsTask();
    public Task AddAppointmentAsync(Appointment appointment, CancellationToken ct) => _db.Appointments.AddAsync(appointment, ct).AsTask();
    public Task AddInvoiceAsync(Invoice invoice, CancellationToken ct) => _db.Invoices.AddAsync(invoice, ct).AsTask();
    public Task AddInsuranceCarrierAsync(InsuranceCarrier carrier, CancellationToken ct) => _db.InsuranceCarriers.AddAsync(carrier, ct).AsTask();
    public Task AddPatientCaseAsync(PatientCase patientCase, CancellationToken ct) => _db.PatientCases.AddAsync(patientCase, ct).AsTask();
    public Task AddCaseChargeAsync(CaseCharge charge, CancellationToken ct) => _db.CaseCharges.AddAsync(charge, ct).AsTask();
    public Task AddChargeCodeTypeAsync(ChargeCodeType chargeCodeType, CancellationToken ct) => _db.ChargeCodeTypes.AddAsync(chargeCodeType, ct).AsTask();
    public Task AddTenantAsync(Tenant tenant, CancellationToken ct) => _db.Tenants.AddAsync(tenant, ct).AsTask();
    public Task AddPracticeLocationAsync(PracticeLocation location, CancellationToken ct) => _db.PracticeLocations.AddAsync(location, ct).AsTask();
    public Task AddProviderScheduleTemplateAsync(ProviderScheduleTemplate template, CancellationToken ct) => _db.ProviderScheduleTemplates.AddAsync(template, ct).AsTask();

    public Task<Patient?> GetPatientAsync(Guid id, CancellationToken ct) => _db.Patients.Include(x => x.Addresses).Include(x => x.InsurancePlans).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Provider?> GetProviderAsync(Guid id, CancellationToken ct) => _db.Providers.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Appointment?> GetAppointmentAsync(Guid id, CancellationToken ct) => _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Invoice?> GetInvoiceAsync(Guid id, CancellationToken ct) => _db.Invoices.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<InsuranceCarrier?> GetInsuranceCarrierAsync(Guid id, CancellationToken ct) => _db.InsuranceCarriers.Include(x => x.Addresses).Include(x => x.SupportedPlans).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<PatientCase?> GetPatientCaseAsync(Guid id, CancellationToken ct) => _db.PatientCases.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<CaseCharge?> GetCaseChargeAsync(Guid id, CancellationToken ct) => _db.CaseCharges.Include(x => x.ChargeCodes).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<ChargeCodeType?> GetChargeCodeTypeAsync(Guid id, CancellationToken ct) => _db.ChargeCodeTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Tenant?> GetTenantAsync(Guid id, CancellationToken ct) => _db.Tenants.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<PracticeLocation?> GetPracticeLocationAsync(Guid id, CancellationToken ct) => _db.PracticeLocations.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<ProviderScheduleTemplate?> GetProviderScheduleTemplateAsync(Guid id, CancellationToken ct) => _db.ProviderScheduleTemplates.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Patient>> ListPatientsAsync(CancellationToken ct) => _db.Patients.Include(x => x.Addresses).Include(x => x.InsurancePlans).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync(ct);
    public Task<List<Provider>> ListProvidersAsync(CancellationToken ct) => _db.Providers.OrderBy(x => x.FullName).ToListAsync(ct);
    public Task<List<Appointment>> ListAppointmentsAsync(CancellationToken ct) => _db.Appointments.OrderBy(x => x.StartTime).ToListAsync(ct);
    public Task<List<Invoice>> ListInvoicesAsync(CancellationToken ct) => _db.Invoices.OrderByDescending(x => x.Id).ToListAsync(ct);
    public Task<List<InsuranceCarrier>> ListInsuranceCarriersAsync(CancellationToken ct) => _db.InsuranceCarriers.Include(x => x.Addresses).Include(x => x.SupportedPlans).OrderBy(x => x.CarrierName).ToListAsync(ct);
    public Task<List<PatientCase>> ListPatientCasesAsync(Guid patientId, CancellationToken ct) => _db.PatientCases.Where(x => x.PatientId == patientId).OrderByDescending(x => x.OpenedDate).ToListAsync(ct);
    public Task<List<CaseCharge>> ListCaseChargesAsync(Guid patientCaseId, CancellationToken ct) => _db.CaseCharges.Include(x => x.ChargeCodes).Where(x => x.PatientCaseId == patientCaseId).OrderByDescending(x => x.DateOfService).ToListAsync(ct);
    public Task<List<ChargeCodeType>> ListChargeCodeTypesAsync(CancellationToken ct) => _db.ChargeCodeTypes.OrderBy(x => x.Name).ToListAsync(ct);
    public Task<List<Tenant>> ListTenantsAsync(CancellationToken ct) => _db.Tenants.Include(x => x.Addresses).OrderBy(x => x.Name).ToListAsync(ct);
    public Task<List<PracticeLocation>> ListPracticeLocationsAsync(Guid tenantId, CancellationToken ct) => _db.PracticeLocations.Include(x => x.Addresses).Where(x => x.TenantId == tenantId).OrderBy(x => x.LocationName).ToListAsync(ct);
    public Task<List<ProviderScheduleTemplate>> ListProviderScheduleTemplatesAsync(Guid providerId, CancellationToken ct) => _db.ProviderScheduleTemplates.Where(x => x.ProviderId == providerId).OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime).ToListAsync(ct);

    public void RemovePatient(Patient patient) => _db.Patients.Remove(patient);
    public void RemoveProvider(Provider provider) => _db.Providers.Remove(provider);
    public void RemoveAppointment(Appointment appointment) => _db.Appointments.Remove(appointment);
    public void RemoveInvoice(Invoice invoice) => _db.Invoices.Remove(invoice);
    public void RemoveInsuranceCarrier(InsuranceCarrier carrier) => _db.InsuranceCarriers.Remove(carrier);
    public void RemovePatientCase(PatientCase patientCase) => _db.PatientCases.Remove(patientCase);
    public void RemoveCaseCharge(CaseCharge charge) => _db.CaseCharges.Remove(charge);
    public void RemoveChargeCodeType(ChargeCodeType chargeCodeType) => _db.ChargeCodeTypes.Remove(chargeCodeType);
    public void RemoveTenant(Tenant tenant) => _db.Tenants.Remove(tenant);
    public void RemovePracticeLocation(PracticeLocation location) => _db.PracticeLocations.Remove(location);
    public void RemoveProviderScheduleTemplate(ProviderScheduleTemplate template) => _db.ProviderScheduleTemplates.Remove(template);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task<DashboardDto> GetDashboardAsync(DateTime dateUtc, CancellationToken ct)
    {
        var patients = await _db.Patients.CountAsync(ct);
        var appointmentsToday = await _db.Appointments.CountAsync(x => x.StartTime.Date == dateUtc.Date && x.Status != AppointmentStatus.Cancelled, ct);
        var gross = await _db.Invoices.SumAsync(x => (decimal?)x.TotalAmount, ct) ?? 0m;
        var paid = await _db.Invoices.SumAsync(x => (decimal?)x.PaidAmount, ct) ?? 0m;
        return new DashboardDto(patients, appointmentsToday, 0, gross, paid, gross - paid);
    }

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}
