using MediTransact.Domain.Entities;

namespace MediTransact.Domain.Interfaces;

public interface IPracticeRepository
{
    Task AddPatientAsync(Patient patient, CancellationToken ct);
    Task AddProviderAsync(Provider provider, CancellationToken ct);
    Task AddAppointmentAsync(Appointment appointment, CancellationToken ct);
    Task AddInvoiceAsync(Invoice invoice, CancellationToken ct);
    Task AddInsuranceCarrierAsync(InsuranceCarrier carrier, CancellationToken ct);
    Task AddPatientCaseAsync(PatientCase patientCase, CancellationToken ct);
    Task AddCaseChargeAsync(CaseCharge charge, CancellationToken ct);
    Task AddChargeCodeTypeAsync(ChargeCodeType chargeCodeType, CancellationToken ct);
    Task AddTenantAsync(Tenant tenant, CancellationToken ct);
    Task AddPracticeLocationAsync(PracticeLocation location, CancellationToken ct);
    Task AddProviderScheduleTemplateAsync(ProviderScheduleTemplate template, CancellationToken ct);

    Task<Patient?> GetPatientAsync(Guid id, CancellationToken ct);
    Task<Provider?> GetProviderAsync(Guid id, CancellationToken ct);
    Task<Appointment?> GetAppointmentAsync(Guid id, CancellationToken ct);
    Task<Invoice?> GetInvoiceAsync(Guid id, CancellationToken ct);
    Task<InsuranceCarrier?> GetInsuranceCarrierAsync(Guid id, CancellationToken ct);
    Task<PatientCase?> GetPatientCaseAsync(Guid id, CancellationToken ct);
    Task<CaseCharge?> GetCaseChargeAsync(Guid id, CancellationToken ct);
    Task<ChargeCodeType?> GetChargeCodeTypeAsync(Guid id, CancellationToken ct);
    Task<Tenant?> GetTenantAsync(Guid id, CancellationToken ct);
    Task<PracticeLocation?> GetPracticeLocationAsync(Guid id, CancellationToken ct);
    Task<ProviderScheduleTemplate?> GetProviderScheduleTemplateAsync(Guid id, CancellationToken ct);

    Task<List<Patient>> ListPatientsAsync(CancellationToken ct);
    Task<List<Provider>> ListProvidersAsync(CancellationToken ct);
    Task<List<Appointment>> ListAppointmentsAsync(CancellationToken ct);
    Task<List<Invoice>> ListInvoicesAsync(CancellationToken ct);
    Task<List<InsuranceCarrier>> ListInsuranceCarriersAsync(CancellationToken ct);
    Task<List<PatientCase>> ListPatientCasesAsync(Guid patientId, CancellationToken ct);
    Task<List<CaseCharge>> ListCaseChargesAsync(Guid patientCaseId, CancellationToken ct);
    Task<List<ChargeCodeType>> ListChargeCodeTypesAsync(CancellationToken ct);
    Task<List<Tenant>> ListTenantsAsync(CancellationToken ct);
    Task<List<PracticeLocation>> ListPracticeLocationsAsync(Guid tenantId, CancellationToken ct);
    Task<List<ProviderScheduleTemplate>> ListProviderScheduleTemplatesAsync(Guid providerId, CancellationToken ct);

    void RemovePatient(Patient patient);
    void RemoveProvider(Provider provider);
    void RemoveAppointment(Appointment appointment);
    void RemoveInvoice(Invoice invoice);
    void RemoveInsuranceCarrier(InsuranceCarrier carrier);
    void RemovePatientCase(PatientCase patientCase);
    void RemoveCaseCharge(CaseCharge charge);
    void RemoveChargeCodeType(ChargeCodeType chargeCodeType);
    void RemoveTenant(Tenant tenant);
    void RemovePracticeLocation(PracticeLocation location);
    void RemoveProviderScheduleTemplate(ProviderScheduleTemplate template);

    Task SaveChangesAsync(CancellationToken ct);
}
