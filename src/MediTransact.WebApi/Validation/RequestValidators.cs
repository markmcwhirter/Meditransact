using FastEndpoints;
using MediTransact.Application.DTOs;
using MediTransact.WebApi.Contracts;

namespace MediTransact.WebApi.Validation;

public sealed class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(128);
    }
}

public sealed class PatientInsurancePlanInputValidator : Validator<PatientInsurancePlanInput>
{
    public PatientInsurancePlanInputValidator()
    {
        RuleFor(x => x.InsuranceCarrierId).NotEmpty();
        RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.GroupNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.MemberNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.AuthorizationContactName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.AuthorizationPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.AuthorizationEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.AuthorizationEndDate)
            .GreaterThanOrEqualTo(x => x.AuthorizationStartDate)
            .When(x => x.AuthorizationEndDate.HasValue)
            .WithMessage("AuthorizationEndDate must be greater than or equal to AuthorizationStartDate.");
    }
}


public sealed class RegisterTenantRequestValidator : Validator<RegisterTenantRequest>
{
    public RegisterTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhysicalAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhysicalAddressLine2).MaximumLength(200);
        RuleFor(x => x.PhysicalCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BillingAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BillingAddressLine2).MaximumLength(200);
        RuleFor(x => x.BillingCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PrimaryPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.SecondaryPhone).MaximumLength(25);
        RuleFor(x => x.PrimaryEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.SecondaryEmail).EmailAddress().MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));
    }
}

public sealed class UpdateTenantRequestValidator : Validator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhysicalAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhysicalAddressLine2).MaximumLength(200);
        RuleFor(x => x.PhysicalCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BillingAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BillingAddressLine2).MaximumLength(200);
        RuleFor(x => x.BillingCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PrimaryPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.SecondaryPhone).MaximumLength(25);
        RuleFor(x => x.PrimaryEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.SecondaryEmail).EmailAddress().MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));
    }
}

public sealed class RegisterPracticeLocationRequestValidator : Validator<RegisterPracticeLocationRequest>
{
    public RegisterPracticeLocationRequestValidator()
    {
        RuleFor(x => x.LocationName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhysicalAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhysicalAddressLine2).MaximumLength(200);
        RuleFor(x => x.PhysicalCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BillingAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BillingAddressLine2).MaximumLength(200);
        RuleFor(x => x.BillingCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PrimaryPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.SecondaryPhone).MaximumLength(25);
        RuleFor(x => x.PrimaryEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.SecondaryEmail).EmailAddress().MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));
    }
}

public sealed class UpdatePracticeLocationRequestValidator : Validator<UpdatePracticeLocationRequest>
{
    public UpdatePracticeLocationRequestValidator()
    {
        RuleFor(x => x.LocationName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhysicalAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhysicalAddressLine2).MaximumLength(200);
        RuleFor(x => x.PhysicalCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhysicalPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BillingAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BillingAddressLine2).MaximumLength(200);
        RuleFor(x => x.BillingCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BillingPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PrimaryPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.SecondaryPhone).MaximumLength(25);
        RuleFor(x => x.PrimaryEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.SecondaryEmail).EmailAddress().MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));
    }
}

public sealed class RegisterPatientRequestValidator : Validator<RegisterPatientRequest>
{
    public RegisterPatientRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Sex).NotEmpty().MaximumLength(30);
        RuleFor(x => x.IdentificationType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.MaritalStatus).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PatientPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.PatientEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.PatientAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PatientAddressLine2).MaximumLength(200);
        RuleFor(x => x.PatientCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PatientState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PatientPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.EmployerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.EmployerPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.EmployerEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Ethnicity).NotEmpty().MaximumLength(80);

        RuleFor(x => x.HipaaConsentDate).NotNull().When(x => x.HipaaConsentAcknowledged);
        RuleFor(x => x.DeceasedDate).NotNull().When(x => x.IsDeceased);
        RuleFor(x => x.DeceasedReason).NotEmpty().When(x => x.IsDeceased);

        RuleFor(x => x.InsurancePlans).NotNull().Must(x => x.Count > 0).WithMessage("At least one insurance plan is required.");
        RuleForEach(x => x.InsurancePlans).SetValidator(new PatientInsurancePlanInputValidator());
    }
}

public sealed class RegisterProviderRequestValidator : Validator<RegisterProviderRequest>
{
    public RegisterProviderRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Npi).NotEmpty().Length(10).Matches("^[0-9]{10}$");
    }
}

public sealed class SupportedPlanRequestValidator : Validator<SupportedPlanRequest>
{
    public SupportedPlanRequestValidator()
    {
        RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PlanName).NotEmpty().MaximumLength(120);
    }
}

public sealed class RegisterInsuranceCarrierRequestValidator : Validator<RegisterInsuranceCarrierRequest>
{
    public RegisterInsuranceCarrierRequestValidator()
    {
        RuleFor(x => x.CarrierName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PayerId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.AuthorizationPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.AuthorizationEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AddressLine2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleForEach(x => x.SupportedPlans).SetValidator(new SupportedPlanRequestValidator());
    }
}

public sealed class UpdateInsuranceCarrierRequestValidator : Validator<UpdateInsuranceCarrierRequest>
{
    public UpdateInsuranceCarrierRequestValidator()
    {
        RuleFor(x => x.CarrierName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PayerId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.AuthorizationPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.AuthorizationEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AddressLine2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleForEach(x => x.SupportedPlans).SetValidator(new SupportedPlanRequestValidator());
    }
}

public sealed class ScheduleAppointmentRequestValidator : Validator<ScheduleAppointmentRequest>
{
    public ScheduleAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.PracticeLocationId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
    }
}

public sealed class CreateInvoiceRequestValidator : Validator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThan(0);
    }
}

public sealed class RecordPaymentRequestValidator : Validator<RecordPaymentRequest>
{
    public RecordPaymentRequestValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class UpdatePatientRequestValidator : Validator<UpdatePatientRequest>
{
    public UpdatePatientRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Sex).NotEmpty().MaximumLength(30);
        RuleFor(x => x.IdentificationType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.MaritalStatus).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PatientPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.PatientEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.PatientAddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PatientAddressLine2).MaximumLength(200);
        RuleFor(x => x.PatientCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PatientState).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PatientPostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.EmployerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.EmployerPhone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.EmployerEmail).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Ethnicity).NotEmpty().MaximumLength(80);

        RuleFor(x => x.HipaaConsentDate).NotNull().When(x => x.HipaaConsentAcknowledged);
        RuleFor(x => x.DeceasedDate).NotNull().When(x => x.IsDeceased);
        RuleFor(x => x.DeceasedReason).NotEmpty().When(x => x.IsDeceased);

        RuleFor(x => x.InsurancePlans).NotNull().Must(x => x.Count > 0).WithMessage("At least one insurance plan is required.");
        RuleForEach(x => x.InsurancePlans).SetValidator(new PatientInsurancePlanInputValidator());
    }
}

public sealed class UpdateProviderRequestValidator : Validator<UpdateProviderRequest>
{
    public UpdateProviderRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Npi).NotEmpty().Length(10).Matches("^[0-9]{10}$");
    }
}

public sealed class UpdateAppointmentRequestValidator : Validator<UpdateAppointmentRequest>
{
    public UpdateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.PracticeLocationId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
    }
}

public sealed class UpdateInvoiceRequestValidator : Validator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThan(0);
    }
}

public sealed class PatientCaseInputValidator : Validator<PatientCaseInput>
{
    public PatientCaseInputValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CaseName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ClosedDate)
            .GreaterThanOrEqualTo(x => x.OpenedDate)
            .When(x => x.ClosedDate.HasValue);
    }
}

public sealed class UpdatePatientCaseRequestValidator : Validator<UpdatePatientCaseRequest>
{
    public UpdatePatientCaseRequestValidator()
    {
        RuleFor(x => x.CaseName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ClosedDate)
            .GreaterThanOrEqualTo(x => x.OpenedDate)
            .When(x => x.ClosedDate.HasValue);
    }
}

public sealed class ChargeCodeInputValidator : Validator<ChargeCodeInput>
{
    public ChargeCodeInputValidator()
    {
        RuleFor(x => x.ChargeCodeTypeId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Units).GreaterThan(0);
    }
}

public sealed class CaseChargeInputValidator : Validator<CaseChargeInput>
{
    public CaseChargeInputValidator()
    {
        RuleFor(x => x.DateOfService).NotEmpty();
        RuleFor(x => x.DateOfEntry).NotEmpty();
        RuleFor(x => x.DateOfBilling)
            .GreaterThanOrEqualTo(x => x.DateOfService)
            .When(x => x.DateOfBilling.HasValue);
        RuleFor(x => x.InsuranceCarrierId).NotEmpty();
        RuleFor(x => x.InsurancePlanCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ChargeCodes).NotNull().Must(x => x.Count > 0);
        RuleForEach(x => x.ChargeCodes).SetValidator(new ChargeCodeInputValidator());
    }
}

public sealed class UpdateCaseChargeRequestValidator : Validator<UpdateCaseChargeRequest>
{
    public UpdateCaseChargeRequestValidator()
    {
        RuleFor(x => x.DateOfService).NotEmpty();
        RuleFor(x => x.DateOfEntry).NotEmpty();
        RuleFor(x => x.DateOfBilling)
            .GreaterThanOrEqualTo(x => x.DateOfService)
            .When(x => x.DateOfBilling.HasValue);
        RuleFor(x => x.InsuranceCarrierId).NotEmpty();
        RuleFor(x => x.InsurancePlanCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ChargeCodes).NotNull().Must(x => x.Count > 0);
        RuleForEach(x => x.ChargeCodes).SetValidator(new ChargeCodeInputValidator());
    }
}

public sealed class ChargeCodeTypeRequestValidator : Validator<ChargeCodeTypeRequest>
{
    public ChargeCodeTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(v => new[] { "CPT", "HCPCS", "ICD9", "ICD10", "SNOMED" }.Contains(v.ToUpperInvariant()))
            .WithMessage("Name must be one of CPT, HCPCS, ICD9, ICD10, or SNOMED.");
    }
}
