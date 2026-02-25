using MediTransact.Domain.Entities;

namespace MediTransact.Application.DTOs;

public record PatientInsurancePlanInput(
    Guid InsuranceCarrierId,
    string PlanCode,
    string GroupNumber,
    string MemberNumber,
    string AuthorizationContactName,
    string AuthorizationPhone,
    string AuthorizationEmail,
    DateOnly AuthorizationStartDate,
    DateOnly? AuthorizationEndDate);

public record PatientInsurancePlanDto(
    Guid InsuranceCarrierId,
    string PlanCode,
    string GroupNumber,
    string MemberNumber,
    string AuthorizationContactName,
    string AuthorizationPhone,
    string AuthorizationEmail,
    DateOnly AuthorizationStartDate,
    DateOnly? AuthorizationEndDate);

public record SupportedPlanRequest(string PlanCode, string PlanName, bool IsActive);
public record SupportedPlanDto(Guid Id, string PlanCode, string PlanName, bool IsActive);

public record RegisterInsuranceCarrierRequest(
    string CarrierName,
    string PayerId,
    string Phone,
    string Email,
    string AuthorizationPhone,
    string AuthorizationEmail,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostalCode,
    List<SupportedPlanRequest> SupportedPlans);

public record UpdateInsuranceCarrierRequest(
    string CarrierName,
    string PayerId,
    string Phone,
    string Email,
    string AuthorizationPhone,
    string AuthorizationEmail,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostalCode,
    List<SupportedPlanRequest> SupportedPlans);

public record InsuranceCarrierDto(
    Guid Id,
    string CarrierName,
    string PayerId,
    string Phone,
    string Email,
    string AuthorizationPhone,
    string AuthorizationEmail,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostalCode,
    List<SupportedPlanDto> SupportedPlans);

public record RegisterPatientRequest(
    string FirstName,
    string MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    string Sex,
    string IdentificationType,
    string IdentificationNumber,
    string MaritalStatus,
    string PatientPhone,
    string PatientEmail,
    string PatientAddressLine1,
    string PatientAddressLine2,
    string PatientCity,
    string PatientState,
    string PatientPostalCode,
    string EmployerName,
    string EmployerPhone,
    string EmployerEmail,
    Guid? PrimaryProviderId,
    bool HipaaConsentAcknowledged,
    DateOnly? HipaaConsentDate,
    string PreferredLanguage,
    string Ethnicity,
    bool IsDeceased,
    DateOnly? DeceasedDate,
    string DeceasedReason,
    List<PatientInsurancePlanInput> InsurancePlans);


public record RegisterTenantRequest(
    string Name,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail);

public record UpdateTenantRequest(
    string Name,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail);

public record TenantDto(
    Guid Id,
    string Name,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail);

public record RegisterPracticeLocationRequest(
    string LocationName,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail,
    bool IsActive);

public record UpdatePracticeLocationRequest(
    string LocationName,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail,
    bool IsActive);

public record PracticeLocationDto(
    Guid Id,
    Guid TenantId,
    string LocationName,
    string PhysicalAddressLine1,
    string PhysicalAddressLine2,
    string PhysicalCity,
    string PhysicalState,
    string PhysicalPostalCode,
    string BillingAddressLine1,
    string BillingAddressLine2,
    string BillingCity,
    string BillingState,
    string BillingPostalCode,
    string PrimaryPhone,
    string SecondaryPhone,
    string PrimaryEmail,
    string SecondaryEmail,
    bool IsActive);

public record RegisterProviderRequest(string FullName, string Specialty, string Npi);
public record ScheduleAppointmentRequest(Guid PatientId, Guid ProviderId, Guid PracticeLocationId, DateTimeOffset StartTime, DateTimeOffset EndTime, string Reason);
public record CreateInvoiceRequest(Guid PatientId, decimal TotalAmount, Guid? AppointmentId);
public record RecordPaymentRequest(Guid InvoiceId, decimal Amount);
public record DashboardDto(int Patients, int AppointmentsToday, int OpenTasks, decimal GrossCharges, decimal PaymentsReceived, decimal OutstandingAr);

public record PatientDto(
    Guid Id,
    string FirstName,
    string MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    string Sex,
    string IdentificationType,
    string IdentificationNumber,
    string MaritalStatus,
    string PatientPhone,
    string PatientEmail,
    string PatientAddressLine1,
    string PatientAddressLine2,
    string PatientCity,
    string PatientState,
    string PatientPostalCode,
    string EmployerName,
    string EmployerPhone,
    string EmployerEmail,
    Guid? PrimaryProviderId,
    bool HipaaConsentAcknowledged,
    DateOnly? HipaaConsentDate,
    string PreferredLanguage,
    string Ethnicity,
    bool IsDeceased,
    DateOnly? DeceasedDate,
    string DeceasedReason,
    List<PatientInsurancePlanDto> InsurancePlans);

public record ProviderDto(Guid Id, string FullName, string Specialty, string Npi);
public record AppointmentDto(Guid Id, Guid PatientId, Guid ProviderId, Guid PracticeLocationId, DateTimeOffset StartTime, DateTimeOffset EndTime, string Reason, AppointmentStatus Status);
public record InvoiceDto(Guid Id, Guid PatientId, Guid? AppointmentId, decimal TotalAmount, decimal PaidAmount, InvoiceStatus Status);

public record UpdatePatientRequest(
    string FirstName,
    string MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    string Sex,
    string IdentificationType,
    string IdentificationNumber,
    string MaritalStatus,
    string PatientPhone,
    string PatientEmail,
    string PatientAddressLine1,
    string PatientAddressLine2,
    string PatientCity,
    string PatientState,
    string PatientPostalCode,
    string EmployerName,
    string EmployerPhone,
    string EmployerEmail,
    Guid? PrimaryProviderId,
    bool HipaaConsentAcknowledged,
    DateOnly? HipaaConsentDate,
    string PreferredLanguage,
    string Ethnicity,
    bool IsDeceased,
    DateOnly? DeceasedDate,
    string DeceasedReason,
    List<PatientInsurancePlanInput> InsurancePlans);

public record UpdateProviderRequest(string FullName, string Specialty, string Npi);
public record UpdateAppointmentRequest(Guid PatientId, Guid ProviderId, Guid PracticeLocationId, DateTimeOffset StartTime, DateTimeOffset EndTime, string Reason, AppointmentStatus Status);
public record UpdateInvoiceRequest(Guid PatientId, decimal TotalAmount, Guid? AppointmentId, InvoiceStatus Status);

public record PatientCaseInput(string CaseNumber, string CaseName, string Description, DateOnly OpenedDate, DateOnly? ClosedDate, bool IsActive);
public record UpdatePatientCaseRequest(string CaseName, string Description, DateOnly OpenedDate, DateOnly? ClosedDate, bool IsActive);
public record PatientCaseDto(Guid Id, Guid PatientId, string CaseNumber, string CaseName, string Description, DateOnly OpenedDate, DateOnly? ClosedDate, bool IsActive);

public record ChargeCodeInput(Guid ChargeCodeTypeId, string Code, decimal Units);
public record CaseChargeInput(
    DateOnly DateOfService,
    DateTimeOffset DateOfEntry,
    DateOnly? DateOfBilling,
    ClaimType ClaimType,
    Guid InsuranceCarrierId,
    string InsurancePlanCode,
    BillingStatus BillingStatus,
    List<ChargeCodeInput> ChargeCodes);

public record UpdateCaseChargeRequest(
    DateOnly DateOfService,
    DateTimeOffset DateOfEntry,
    DateOnly? DateOfBilling,
    ClaimType ClaimType,
    Guid InsuranceCarrierId,
    string InsurancePlanCode,
    BillingStatus BillingStatus,
    List<ChargeCodeInput> ChargeCodes);

public record CaseChargeCodeDto(Guid Id, Guid ChargeCodeTypeId, string Code, decimal Units);
public record CaseChargeDto(
    Guid Id,
    Guid PatientCaseId,
    DateOnly DateOfService,
    DateTimeOffset DateOfEntry,
    DateOnly? DateOfBilling,
    ClaimType ClaimType,
    Guid InsuranceCarrierId,
    string InsurancePlanCode,
    BillingStatus BillingStatus,
    List<CaseChargeCodeDto> ChargeCodes);

public record ChargeCodeTypeRequest(string Name, bool IsActive);
public record ChargeCodeTypeDto(Guid Id, string Name, bool IsActive);
