using MediTransact.Application.DTOs;
using MediTransact.Application.Errors;
using OneOf;
using OneOf.Types;

namespace MediTransact.Application.Interfaces;

public interface IPracticeService
{
    Task<OneOf<Guid, AppError>> RegisterPatientAsync(RegisterPatientRequest request, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreateTenantAsync(RegisterTenantRequest request, CancellationToken ct);
    Task<OneOf<List<TenantDto>, AppError>> ListTenantsAsync(CancellationToken ct);
    Task<OneOf<TenantDto, AppError>> GetTenantByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateTenantAsync(Guid id, UpdateTenantRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteTenantAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreatePracticeLocationAsync(Guid tenantId, RegisterPracticeLocationRequest request, CancellationToken ct);
    Task<OneOf<List<PracticeLocationDto>, AppError>> ListPracticeLocationsAsync(Guid tenantId, CancellationToken ct);
    Task<OneOf<PracticeLocationDto, AppError>> GetPracticeLocationByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdatePracticeLocationAsync(Guid id, UpdatePracticeLocationRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeletePracticeLocationAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> RegisterProviderAsync(RegisterProviderRequest request, CancellationToken ct);
    Task<OneOf<Guid, AppError>> CreateProviderScheduleTemplateAsync(Guid providerId, CreateProviderScheduleTemplateRequest request, CancellationToken ct);
    Task<OneOf<List<ProviderScheduleTemplateDto>, AppError>> ListProviderScheduleTemplatesAsync(Guid providerId, CancellationToken ct);
    Task<OneOf<ProviderScheduleTemplateDto, AppError>> GetProviderScheduleTemplateByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateProviderScheduleTemplateAsync(Guid id, UpdateProviderScheduleTemplateRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteProviderScheduleTemplateAsync(Guid id, CancellationToken ct);
    Task<OneOf<Guid, AppError>> ScheduleAppointmentAsync(ScheduleAppointmentRequest request, CancellationToken ct);
    Task<OneOf<Guid, AppError>> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken ct);
    Task<OneOf<DashboardDto, AppError>> GetDashboardAsync(CancellationToken ct);

    Task<OneOf<List<PatientDto>, AppError>> ListPatientsAsync(CancellationToken ct);
    Task<OneOf<PatientDto, AppError>> GetPatientByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdatePatientAsync(Guid id, UpdatePatientRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeletePatientAsync(Guid id, CancellationToken ct);

    Task<OneOf<List<ProviderDto>, AppError>> ListProvidersAsync(CancellationToken ct);
    Task<OneOf<ProviderDto, AppError>> GetProviderByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateProviderAsync(Guid id, UpdateProviderRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteProviderAsync(Guid id, CancellationToken ct);

    Task<OneOf<List<AppointmentDto>, AppError>> ListAppointmentsAsync(CancellationToken ct);
    Task<OneOf<AppointmentDto, AppError>> GetAppointmentByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteAppointmentAsync(Guid id, CancellationToken ct);

    Task<OneOf<List<InvoiceDto>, AppError>> ListInvoicesAsync(CancellationToken ct);
    Task<OneOf<InvoiceDto, AppError>> GetInvoiceByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteInvoiceAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreateInsuranceCarrierAsync(RegisterInsuranceCarrierRequest request, CancellationToken ct);
    Task<OneOf<List<InsuranceCarrierDto>, AppError>> ListInsuranceCarriersAsync(CancellationToken ct);
    Task<OneOf<InsuranceCarrierDto, AppError>> GetInsuranceCarrierByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateInsuranceCarrierAsync(Guid id, UpdateInsuranceCarrierRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteInsuranceCarrierAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreatePatientCaseAsync(Guid patientId, PatientCaseInput request, CancellationToken ct);
    Task<OneOf<List<PatientCaseDto>, AppError>> ListPatientCasesAsync(Guid patientId, CancellationToken ct);
    Task<OneOf<PatientCaseDto, AppError>> GetPatientCaseByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdatePatientCaseAsync(Guid id, UpdatePatientCaseRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeletePatientCaseAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreateCaseChargeAsync(Guid patientCaseId, CaseChargeInput request, CancellationToken ct);
    Task<OneOf<List<CaseChargeDto>, AppError>> ListCaseChargesAsync(Guid patientCaseId, CancellationToken ct);
    Task<OneOf<CaseChargeDto, AppError>> GetCaseChargeByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateCaseChargeAsync(Guid id, UpdateCaseChargeRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteCaseChargeAsync(Guid id, CancellationToken ct);

    Task<OneOf<Guid, AppError>> CreateChargeCodeTypeAsync(ChargeCodeTypeRequest request, CancellationToken ct);
    Task<OneOf<List<ChargeCodeTypeDto>, AppError>> ListChargeCodeTypesAsync(CancellationToken ct);
    Task<OneOf<ChargeCodeTypeDto, AppError>> GetChargeCodeTypeByIdAsync(Guid id, CancellationToken ct);
    Task<OneOf<Success, AppError>> UpdateChargeCodeTypeAsync(Guid id, ChargeCodeTypeRequest request, CancellationToken ct);
    Task<OneOf<Success, AppError>> DeleteChargeCodeTypeAsync(Guid id, CancellationToken ct);
}
