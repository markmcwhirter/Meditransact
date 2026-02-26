using MediTransact.Application.DTOs;
using MediTransact.Application.Errors;
using MediTransact.Application.Interfaces;
using MediTransact.Domain.Entities;
using MediTransact.Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace MediTransact.Application.Services;

public class PracticeService(IPracticeRepository repository, IPracticeReadModel readModel) : IPracticeService
{
    public async Task<OneOf<Guid, AppError>> RegisterPatientAsync(RegisterPatientRequest request, CancellationToken ct)
    {
        if (request.PrimaryProviderId.HasValue && await repository.GetProviderAsync(request.PrimaryProviderId.Value, ct) is null)
            return NotFound("provider.not_found", "Primary provider not found");

        foreach (var p in request.InsurancePlans)
        {
            if (await repository.GetInsuranceCarrierAsync(p.InsuranceCarrierId, ct) is null)
                return NotFound("insurance_carrier.not_found", $"Insurance carrier {p.InsuranceCarrierId} not found");
        }

        var patient = new Patient(
            request.FirstName, request.MiddleName, request.LastName, request.DateOfBirth, request.Sex,
            request.IdentificationType, request.IdentificationNumber, request.MaritalStatus,
            request.PatientEmail,
            request.EmployerName, request.EmployerPhone, request.EmployerEmail,
            request.PrimaryProviderId, request.HipaaConsentAcknowledged, request.HipaaConsentDate,
            request.PreferredLanguage, request.Ethnicity,
            request.IsDeceased, request.DeceasedDate, request.DeceasedReason,
            [CreateAddress(AddressType.Physical, request.PatientAddressLine1, request.PatientAddressLine2, string.Empty, request.PatientCity, request.PatientState, request.PatientPostalCode, request.PatientPhone, PhoneType.Primary)],
            request.InsurancePlans.Select(Map));

        await repository.AddPatientAsync(patient, ct);
        await repository.SaveChangesAsync(ct);
        return patient.Id;
    }

    public async Task<OneOf<Guid, AppError>> RegisterProviderAsync(RegisterProviderRequest request, CancellationToken ct)
    {
        var provider = new Provider(request.FullName, request.Specialty, request.Npi);
        await repository.AddProviderAsync(provider, ct);
        await repository.SaveChangesAsync(ct);
        return provider.Id;
    }


    public async Task<OneOf<Guid, AppError>> CreateProviderScheduleTemplateAsync(Guid providerId, CreateProviderScheduleTemplateRequest request, CancellationToken ct)
    {
        if (await repository.GetProviderAsync(providerId, ct) is null) return NotFound("provider.not_found", "Provider not found");
        if (await repository.GetPracticeLocationAsync(request.PracticeLocationId, ct) is null) return NotFound("practice_location.not_found", "Practice location not found");

        var template = new ProviderScheduleTemplate(
            providerId,
            request.PracticeLocationId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.EffectiveStartDate,
            request.EffectiveEndDate,
            request.IsActive);

        await repository.AddProviderScheduleTemplateAsync(template, ct);
        await repository.SaveChangesAsync(ct);
        return template.Id;
    }

    public async Task<OneOf<List<ProviderScheduleTemplateDto>, AppError>> ListProviderScheduleTemplatesAsync(Guid providerId, CancellationToken ct)
    {
        if (await repository.GetProviderAsync(providerId, ct) is null) return NotFound("provider.not_found", "Provider not found");
        return (await repository.ListProviderScheduleTemplatesAsync(providerId, ct)).Select(Map).ToList();
    }

    public async Task<OneOf<ProviderScheduleTemplateDto, AppError>> GetProviderScheduleTemplateByIdAsync(Guid id, CancellationToken ct)
    {
        var template = await repository.GetProviderScheduleTemplateAsync(id, ct);
        return template is null ? NotFound("provider_schedule_template.not_found", "Provider schedule template not found") : Map(template);
    }

    public async Task<OneOf<Success, AppError>> UpdateProviderScheduleTemplateAsync(Guid id, UpdateProviderScheduleTemplateRequest request, CancellationToken ct)
    {
        var template = await repository.GetProviderScheduleTemplateAsync(id, ct);
        if (template is null) return NotFound("provider_schedule_template.not_found", "Provider schedule template not found");
        if (await repository.GetPracticeLocationAsync(request.PracticeLocationId, ct) is null) return NotFound("practice_location.not_found", "Practice location not found");

        template.Update(
            request.PracticeLocationId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.EffectiveStartDate,
            request.EffectiveEndDate,
            request.IsActive);

        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeleteProviderScheduleTemplateAsync(Guid id, CancellationToken ct)
    {
        var template = await repository.GetProviderScheduleTemplateAsync(id, ct);
        if (template is null) return NotFound("provider_schedule_template.not_found", "Provider schedule template not found");
        repository.RemoveProviderScheduleTemplate(template);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Guid, AppError>> ScheduleAppointmentAsync(ScheduleAppointmentRequest request, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(request.PatientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        if (await repository.GetProviderAsync(request.ProviderId, ct) is null) return NotFound("provider.not_found", "Provider not found");
        if (await repository.GetPracticeLocationAsync(request.PracticeLocationId, ct) is null) return NotFound("practice_location.not_found", "Practice location not found");

        var templates = await repository.ListProviderScheduleTemplatesAsync(request.ProviderId, ct);
        var hasCoverage = templates.Any(x => x.IsActive && !x.IsDeleted && x.PracticeLocationId == request.PracticeLocationId && x.Covers(request.StartTime, request.EndTime));
        if (!hasCoverage) return NotFound("provider_schedule_template.not_available", "No active practitioner schedule template covers this appointment window at the location");

        var appt = new Appointment(request.PatientId, request.ProviderId, request.PracticeLocationId, request.StartTime, request.EndTime, request.Reason);
        await repository.AddAppointmentAsync(appt, ct);
        await repository.SaveChangesAsync(ct);
        return appt.Id;
    }

    public async Task<OneOf<Guid, AppError>> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(request.PatientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        var invoice = new Invoice(request.PatientId, request.TotalAmount, request.AppointmentId);
        await repository.AddInvoiceAsync(invoice, ct);
        await repository.SaveChangesAsync(ct);
        return invoice.Id;
    }

    public async Task<OneOf<Success, AppError>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken ct)
    {
        var invoice = await repository.GetInvoiceAsync(request.InvoiceId, ct);
        if (invoice is null) return NotFound("invoice.not_found", "Invoice not found");
        invoice.ApplyPayment(request.Amount);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<DashboardDto, AppError>> GetDashboardAsync(CancellationToken ct) =>
        await readModel.GetDashboardAsync(DateTime.UtcNow.Date, ct);

    public async Task<OneOf<Guid, AppError>> CreateTenantAsync(RegisterTenantRequest request, CancellationToken ct)
    {
        var tenant = new Tenant(
            request.Name,
            request.PrimaryEmail,
            request.SecondaryEmail,
            [
                CreateAddress(AddressType.Physical, request.PhysicalAddressLine1, request.PhysicalAddressLine2, string.Empty, request.PhysicalCity, request.PhysicalState, request.PhysicalPostalCode, request.PrimaryPhone, PhoneType.Primary),
                CreateAddress(AddressType.Billing, request.BillingAddressLine1, request.BillingAddressLine2, string.Empty, request.BillingCity, request.BillingState, request.BillingPostalCode, request.SecondaryPhone, PhoneType.Secondary)
            ]);

        await repository.AddTenantAsync(tenant, ct);
        await repository.SaveChangesAsync(ct);
        return tenant.Id;
    }

    public async Task<OneOf<List<TenantDto>, AppError>> ListTenantsAsync(CancellationToken ct) =>
        (await repository.ListTenantsAsync(ct)).Select(Map).ToList();

    public async Task<OneOf<TenantDto, AppError>> GetTenantByIdAsync(Guid id, CancellationToken ct)
    {
        var tenant = await repository.GetTenantAsync(id, ct);
        return tenant is null ? NotFound("tenant.not_found", "Tenant not found") : Map(tenant);
    }

    public async Task<OneOf<Success, AppError>> UpdateTenantAsync(Guid id, UpdateTenantRequest request, CancellationToken ct)
    {
        var tenant = await repository.GetTenantAsync(id, ct);
        if (tenant is null) return NotFound("tenant.not_found", "Tenant not found");

        tenant.Update(
            request.Name,
            request.PrimaryEmail,
            request.SecondaryEmail,
            [
                CreateAddress(AddressType.Physical, request.PhysicalAddressLine1, request.PhysicalAddressLine2, string.Empty, request.PhysicalCity, request.PhysicalState, request.PhysicalPostalCode, request.PrimaryPhone, PhoneType.Primary),
                CreateAddress(AddressType.Billing, request.BillingAddressLine1, request.BillingAddressLine2, string.Empty, request.BillingCity, request.BillingState, request.BillingPostalCode, request.SecondaryPhone, PhoneType.Secondary)
            ]);

        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeleteTenantAsync(Guid id, CancellationToken ct)
    {
        var tenant = await repository.GetTenantAsync(id, ct);
        if (tenant is null) return NotFound("tenant.not_found", "Tenant not found");
        repository.RemoveTenant(tenant);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Guid, AppError>> CreatePracticeLocationAsync(Guid tenantId, RegisterPracticeLocationRequest request, CancellationToken ct)
    {
        if (await repository.GetTenantAsync(tenantId, ct) is null) return NotFound("tenant.not_found", "Tenant not found");

        var location = new PracticeLocation(
            tenantId,
            request.LocationName,
            request.PrimaryEmail,
            request.SecondaryEmail,
            request.IsActive,
            [
                CreateAddress(AddressType.Physical, request.PhysicalAddressLine1, request.PhysicalAddressLine2, string.Empty, request.PhysicalCity, request.PhysicalState, request.PhysicalPostalCode, request.PrimaryPhone, PhoneType.Primary),
                CreateAddress(AddressType.Billing, request.BillingAddressLine1, request.BillingAddressLine2, string.Empty, request.BillingCity, request.BillingState, request.BillingPostalCode, request.SecondaryPhone, PhoneType.Secondary)
            ]);
        await repository.AddPracticeLocationAsync(location, ct);
        await repository.SaveChangesAsync(ct);
        return location.Id;
    }

    public async Task<OneOf<List<PracticeLocationDto>, AppError>> ListPracticeLocationsAsync(Guid tenantId, CancellationToken ct) =>
        (await repository.ListPracticeLocationsAsync(tenantId, ct)).Select(Map).ToList();

    public async Task<OneOf<PracticeLocationDto, AppError>> GetPracticeLocationByIdAsync(Guid id, CancellationToken ct)
    {
        var location = await repository.GetPracticeLocationAsync(id, ct);
        return location is null ? NotFound("practice_location.not_found", "Practice location not found") : Map(location);
    }

    public async Task<OneOf<Success, AppError>> UpdatePracticeLocationAsync(Guid id, UpdatePracticeLocationRequest request, CancellationToken ct)
    {
        var location = await repository.GetPracticeLocationAsync(id, ct);
        if (location is null) return NotFound("practice_location.not_found", "Practice location not found");

        location.Update(
            request.LocationName,
            request.PrimaryEmail,
            request.SecondaryEmail,
            request.IsActive,
            [
                CreateAddress(AddressType.Physical, request.PhysicalAddressLine1, request.PhysicalAddressLine2, string.Empty, request.PhysicalCity, request.PhysicalState, request.PhysicalPostalCode, request.PrimaryPhone, PhoneType.Primary),
                CreateAddress(AddressType.Billing, request.BillingAddressLine1, request.BillingAddressLine2, string.Empty, request.BillingCity, request.BillingState, request.BillingPostalCode, request.SecondaryPhone, PhoneType.Secondary)
            ]);

        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeletePracticeLocationAsync(Guid id, CancellationToken ct)
    {
        var location = await repository.GetPracticeLocationAsync(id, ct);
        if (location is null) return NotFound("practice_location.not_found", "Practice location not found");
        repository.RemovePracticeLocation(location);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<List<PatientDto>, AppError>> ListPatientsAsync(CancellationToken ct) =>
        (await repository.ListPatientsAsync(ct)).Select(Map).ToList();

    public async Task<OneOf<PatientDto, AppError>> GetPatientByIdAsync(Guid id, CancellationToken ct)
    {
        var patient = await repository.GetPatientAsync(id, ct);
        return patient is null ? NotFound("patient.not_found", "Patient not found") : Map(patient);
    }

    public async Task<OneOf<Success, AppError>> UpdatePatientAsync(Guid id, UpdatePatientRequest request, CancellationToken ct)
    {
        if (request.PrimaryProviderId.HasValue && await repository.GetProviderAsync(request.PrimaryProviderId.Value, ct) is null)
            return NotFound("provider.not_found", "Primary provider not found");

        foreach (var p in request.InsurancePlans)
        {
            if (await repository.GetInsuranceCarrierAsync(p.InsuranceCarrierId, ct) is null)
                return NotFound("insurance_carrier.not_found", $"Insurance carrier {p.InsuranceCarrierId} not found");
        }

        var patient = await repository.GetPatientAsync(id, ct);
        if (patient is null) return NotFound("patient.not_found", "Patient not found");

        patient.Update(
            request.FirstName, request.MiddleName, request.LastName, request.DateOfBirth, request.Sex,
            request.IdentificationType, request.IdentificationNumber, request.MaritalStatus,
            request.PatientEmail,
            request.EmployerName, request.EmployerPhone, request.EmployerEmail,
            request.PrimaryProviderId, request.HipaaConsentAcknowledged, request.HipaaConsentDate,
            request.PreferredLanguage, request.Ethnicity,
            request.IsDeceased, request.DeceasedDate, request.DeceasedReason,
            [CreateAddress(AddressType.Physical, request.PatientAddressLine1, request.PatientAddressLine2, string.Empty, request.PatientCity, request.PatientState, request.PatientPostalCode, request.PatientPhone, PhoneType.Primary)],
            request.InsurancePlans.Select(Map));

        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeletePatientAsync(Guid id, CancellationToken ct)
    {
        var patient = await repository.GetPatientAsync(id, ct);
        if (patient is null) return NotFound("patient.not_found", "Patient not found");
        repository.RemovePatient(patient);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<List<ProviderDto>, AppError>> ListProvidersAsync(CancellationToken ct) => (await repository.ListProvidersAsync(ct)).Select(Map).ToList();
    public async Task<OneOf<ProviderDto, AppError>> GetProviderByIdAsync(Guid id, CancellationToken ct) =>
        await repository.GetProviderAsync(id, ct) is { } p ? Map(p) : NotFound("provider.not_found", "Provider not found");
    public async Task<OneOf<Success, AppError>> UpdateProviderAsync(Guid id, UpdateProviderRequest request, CancellationToken ct)
    {
        var provider = await repository.GetProviderAsync(id, ct);
        if (provider is null) return NotFound("provider.not_found", "Provider not found");
        provider.Update(request.FullName, request.Specialty, request.Npi);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }
    public async Task<OneOf<Success, AppError>> DeleteProviderAsync(Guid id, CancellationToken ct)
    {
        var provider = await repository.GetProviderAsync(id, ct);
        if (provider is null) return NotFound("provider.not_found", "Provider not found");
        repository.RemoveProvider(provider);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<List<AppointmentDto>, AppError>> ListAppointmentsAsync(CancellationToken ct) => (await repository.ListAppointmentsAsync(ct)).Select(Map).ToList();
    public async Task<OneOf<AppointmentDto, AppError>> GetAppointmentByIdAsync(Guid id, CancellationToken ct) =>
        await repository.GetAppointmentAsync(id, ct) is { } a ? Map(a) : NotFound("appointment.not_found", "Appointment not found");
    public async Task<OneOf<Success, AppError>> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(request.PatientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        if (await repository.GetProviderAsync(request.ProviderId, ct) is null) return NotFound("provider.not_found", "Provider not found");
        if (await repository.GetPracticeLocationAsync(request.PracticeLocationId, ct) is null) return NotFound("practice_location.not_found", "Practice location not found");

        var templates = await repository.ListProviderScheduleTemplatesAsync(request.ProviderId, ct);
        var hasCoverage = templates.Any(x => x.IsActive && !x.IsDeleted && x.PracticeLocationId == request.PracticeLocationId && x.Covers(request.StartTime, request.EndTime));
        if (!hasCoverage) return NotFound("provider_schedule_template.not_available", "No active practitioner schedule template covers this appointment window at the location");

        var appointment = await repository.GetAppointmentAsync(id, ct);
        if (appointment is null) return NotFound("appointment.not_found", "Appointment not found");
        appointment.Update(request.PatientId, request.ProviderId, request.PracticeLocationId, request.StartTime, request.EndTime, request.Reason, request.Status);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }
    public async Task<OneOf<Success, AppError>> DeleteAppointmentAsync(Guid id, CancellationToken ct)
    {
        var appointment = await repository.GetAppointmentAsync(id, ct);
        if (appointment is null) return NotFound("appointment.not_found", "Appointment not found");
        repository.RemoveAppointment(appointment);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<List<InvoiceDto>, AppError>> ListInvoicesAsync(CancellationToken ct) => (await repository.ListInvoicesAsync(ct)).Select(Map).ToList();
    public async Task<OneOf<InvoiceDto, AppError>> GetInvoiceByIdAsync(Guid id, CancellationToken ct) =>
        await repository.GetInvoiceAsync(id, ct) is { } i ? Map(i) : NotFound("invoice.not_found", "Invoice not found");
    public async Task<OneOf<Success, AppError>> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest request, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(request.PatientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        var invoice = await repository.GetInvoiceAsync(id, ct);
        if (invoice is null) return NotFound("invoice.not_found", "Invoice not found");
        invoice.Update(request.PatientId, request.TotalAmount, request.AppointmentId, request.Status);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }
    public async Task<OneOf<Success, AppError>> DeleteInvoiceAsync(Guid id, CancellationToken ct)
    {
        var invoice = await repository.GetInvoiceAsync(id, ct);
        if (invoice is null) return NotFound("invoice.not_found", "Invoice not found");
        repository.RemoveInvoice(invoice);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Guid, AppError>> CreateInsuranceCarrierAsync(RegisterInsuranceCarrierRequest request, CancellationToken ct)
    {
        var carrier = new InsuranceCarrier(
            request.CarrierName, request.PayerId, request.Email,
            request.AuthorizationPhone, request.AuthorizationEmail,
            [CreateAddress(AddressType.Physical, request.AddressLine1, request.AddressLine2, string.Empty, request.City, request.State, request.PostalCode, request.Phone, PhoneType.Primary)],
            request.SupportedPlans.Select(x => new SupportedInsurancePlan(x.PlanCode, x.PlanName, x.IsActive)));
        await repository.AddInsuranceCarrierAsync(carrier, ct);
        await repository.SaveChangesAsync(ct);
        return carrier.Id;
    }

    public async Task<OneOf<List<InsuranceCarrierDto>, AppError>> ListInsuranceCarriersAsync(CancellationToken ct) =>
        (await repository.ListInsuranceCarriersAsync(ct)).Select(Map).ToList();

    public async Task<OneOf<InsuranceCarrierDto, AppError>> GetInsuranceCarrierByIdAsync(Guid id, CancellationToken ct)
    {
        var carrier = await repository.GetInsuranceCarrierAsync(id, ct);
        return carrier is null ? NotFound("insurance_carrier.not_found", "Insurance carrier not found") : Map(carrier);
    }

    public async Task<OneOf<Success, AppError>> UpdateInsuranceCarrierAsync(Guid id, UpdateInsuranceCarrierRequest request, CancellationToken ct)
    {
        var carrier = await repository.GetInsuranceCarrierAsync(id, ct);
        if (carrier is null) return NotFound("insurance_carrier.not_found", "Insurance carrier not found");

        carrier.Update(
            request.CarrierName, request.PayerId, request.Email,
            request.AuthorizationPhone, request.AuthorizationEmail,
            [CreateAddress(AddressType.Physical, request.AddressLine1, request.AddressLine2, string.Empty, request.City, request.State, request.PostalCode, request.Phone, PhoneType.Primary)],
            request.SupportedPlans.Select(x => new SupportedInsurancePlan(x.PlanCode, x.PlanName, x.IsActive)));

        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeleteInsuranceCarrierAsync(Guid id, CancellationToken ct)
    {
        var carrier = await repository.GetInsuranceCarrierAsync(id, ct);
        if (carrier is null) return NotFound("insurance_carrier.not_found", "Insurance carrier not found");
        repository.RemoveInsuranceCarrier(carrier);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    private static PatientInsurancePlan Map(PatientInsurancePlanInput p) =>
        new(p.InsuranceCarrierId, p.PlanCode, p.GroupNumber, p.MemberNumber, p.AuthorizationContactName, p.AuthorizationPhone, p.AuthorizationEmail, p.AuthorizationStartDate, p.AuthorizationEndDate);

    private static PatientInsurancePlanDto Map(PatientInsurancePlan p) =>
        new(p.InsuranceCarrierId, p.PlanCode, p.GroupNumber, p.MemberNumber, p.AuthorizationContactName, p.AuthorizationPhone, p.AuthorizationEmail, p.AuthorizationStartDate, p.AuthorizationEndDate);

    private static CaseChargeCode Map(ChargeCodeInput c) => new(c.ChargeCodeTypeId, c.Code, c.Units);

    private static CaseChargeCodeDto Map(CaseChargeCode c) => new(c.Id, c.ChargeCodeTypeId, c.Code, c.Units);

    private static PatientCaseDto Map(PatientCase c) => new(c.Id, c.PatientId, c.CaseNumber, c.CaseName, c.Description, c.OpenedDate, c.ClosedDate, c.IsActive);

    private static CaseChargeDto Map(CaseCharge c) => new(c.Id, c.PatientCaseId, c.DateOfService, c.DateOfEntry, c.DateOfBilling, c.ClaimType, c.InsuranceCarrierId, c.InsurancePlanCode, c.BillingStatus, c.ChargeCodes.Select(Map).ToList());

    private static ChargeCodeTypeDto Map(ChargeCodeType c) => new(c.Id, c.Name, c.IsActive);



    public async Task<OneOf<Guid, AppError>> CreatePatientCaseAsync(Guid patientId, PatientCaseInput request, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(patientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        var patientCase = new PatientCase(patientId, request.CaseNumber, request.CaseName, request.Description, request.OpenedDate, request.ClosedDate, request.IsActive);
        await repository.AddPatientCaseAsync(patientCase, ct);
        await repository.SaveChangesAsync(ct);
        return patientCase.Id;
    }

    public async Task<OneOf<List<PatientCaseDto>, AppError>> ListPatientCasesAsync(Guid patientId, CancellationToken ct)
    {
        if (await repository.GetPatientAsync(patientId, ct) is null) return NotFound("patient.not_found", "Patient not found");
        return (await repository.ListPatientCasesAsync(patientId, ct)).Select(Map).ToList();
    }

    public async Task<OneOf<PatientCaseDto, AppError>> GetPatientCaseByIdAsync(Guid id, CancellationToken ct)
    {
        var patientCase = await repository.GetPatientCaseAsync(id, ct);
        return patientCase is null ? NotFound("patient_case.not_found", "Patient case not found") : Map(patientCase);
    }

    public async Task<OneOf<Success, AppError>> UpdatePatientCaseAsync(Guid id, UpdatePatientCaseRequest request, CancellationToken ct)
    {
        var patientCase = await repository.GetPatientCaseAsync(id, ct);
        if (patientCase is null) return NotFound("patient_case.not_found", "Patient case not found");
        patientCase.Update(request.CaseName, request.Description, request.OpenedDate, request.ClosedDate, request.IsActive);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeletePatientCaseAsync(Guid id, CancellationToken ct)
    {
        var patientCase = await repository.GetPatientCaseAsync(id, ct);
        if (patientCase is null) return NotFound("patient_case.not_found", "Patient case not found");
        repository.RemovePatientCase(patientCase);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Guid, AppError>> CreateCaseChargeAsync(Guid patientCaseId, CaseChargeInput request, CancellationToken ct)
    {
        if (await repository.GetPatientCaseAsync(patientCaseId, ct) is null) return NotFound("patient_case.not_found", "Patient case not found");
        if (await repository.GetInsuranceCarrierAsync(request.InsuranceCarrierId, ct) is null) return NotFound("insurance_carrier.not_found", "Insurance carrier not found");
        foreach (var code in request.ChargeCodes)
            if (await repository.GetChargeCodeTypeAsync(code.ChargeCodeTypeId, ct) is null)
                return NotFound("charge_code_type.not_found", $"Charge code type {code.ChargeCodeTypeId} not found");

        var charge = new CaseCharge(request.DateOfService, request.DateOfEntry, request.DateOfBilling, request.ClaimType, request.InsuranceCarrierId, request.InsurancePlanCode, request.BillingStatus, request.ChargeCodes.Select(Map));
        await repository.AddCaseChargeAsync(charge, ct);
        await repository.SaveChangesAsync(ct);
        return charge.Id;
    }

    public async Task<OneOf<List<CaseChargeDto>, AppError>> ListCaseChargesAsync(Guid patientCaseId, CancellationToken ct)
    {
        if (await repository.GetPatientCaseAsync(patientCaseId, ct) is null) return NotFound("patient_case.not_found", "Patient case not found");
        return (await repository.ListCaseChargesAsync(patientCaseId, ct)).Select(Map).ToList();
    }

    public async Task<OneOf<CaseChargeDto, AppError>> GetCaseChargeByIdAsync(Guid id, CancellationToken ct)
    {
        var charge = await repository.GetCaseChargeAsync(id, ct);
        return charge is null ? NotFound("case_charge.not_found", "Case charge not found") : Map(charge);
    }

    public async Task<OneOf<Success, AppError>> UpdateCaseChargeAsync(Guid id, UpdateCaseChargeRequest request, CancellationToken ct)
    {
        var charge = await repository.GetCaseChargeAsync(id, ct);
        if (charge is null) return NotFound("case_charge.not_found", "Case charge not found");
        if (await repository.GetInsuranceCarrierAsync(request.InsuranceCarrierId, ct) is null) return NotFound("insurance_carrier.not_found", "Insurance carrier not found");
        foreach (var code in request.ChargeCodes)
            if (await repository.GetChargeCodeTypeAsync(code.ChargeCodeTypeId, ct) is null)
                return NotFound("charge_code_type.not_found", $"Charge code type {code.ChargeCodeTypeId} not found");

        charge.Update(request.DateOfService, request.DateOfEntry, request.DateOfBilling, request.ClaimType, request.InsuranceCarrierId, request.InsurancePlanCode, request.BillingStatus, request.ChargeCodes.Select(Map));
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeleteCaseChargeAsync(Guid id, CancellationToken ct)
    {
        var charge = await repository.GetCaseChargeAsync(id, ct);
        if (charge is null) return NotFound("case_charge.not_found", "Case charge not found");
        repository.RemoveCaseCharge(charge);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Guid, AppError>> CreateChargeCodeTypeAsync(ChargeCodeTypeRequest request, CancellationToken ct)
    {
        var chargeCodeType = new ChargeCodeType(request.Name, request.IsActive);
        await repository.AddChargeCodeTypeAsync(chargeCodeType, ct);
        await repository.SaveChangesAsync(ct);
        return chargeCodeType.Id;
    }

    public async Task<OneOf<List<ChargeCodeTypeDto>, AppError>> ListChargeCodeTypesAsync(CancellationToken ct) =>
        (await repository.ListChargeCodeTypesAsync(ct)).Select(Map).ToList();

    public async Task<OneOf<ChargeCodeTypeDto, AppError>> GetChargeCodeTypeByIdAsync(Guid id, CancellationToken ct)
    {
        var chargeCodeType = await repository.GetChargeCodeTypeAsync(id, ct);
        if (chargeCodeType is null) return NotFound("charge_code_type.not_found", $"Charge code type {id} not found");
        return Map(chargeCodeType);
    }

    public async Task<OneOf<Success, AppError>> UpdateChargeCodeTypeAsync(Guid id, ChargeCodeTypeRequest request, CancellationToken ct)
    {
        var chargeCodeType = await repository.GetChargeCodeTypeAsync(id, ct);
        if (chargeCodeType is null) return NotFound("charge_code_type.not_found", "Charge code type not found");
        chargeCodeType.Update(request.Name, request.IsActive);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    public async Task<OneOf<Success, AppError>> DeleteChargeCodeTypeAsync(Guid id, CancellationToken ct)
    {
        var chargeCodeType = await repository.GetChargeCodeTypeAsync(id, ct);
        if (chargeCodeType is null) return NotFound("charge_code_type.not_found", "Charge code type not found");
        repository.RemoveChargeCodeType(chargeCodeType);
        await repository.SaveChangesAsync(ct);
        return new Success();
    }

    private static AppError NotFound(string code, string message) => new(ErrorKind.NotFound, code, message);

    private static PatientDto Map(Patient p) => new(
        p.Id, p.FirstName, p.MiddleName, p.LastName, p.DateOfBirth, p.Sex, p.IdentificationType, p.IdentificationNumber,
        p.MaritalStatus, GetAddressPhone(p.Addresses, AddressType.Physical, PhoneType.Primary), p.PatientEmail, GetAddressLine1(p.Addresses, AddressType.Physical), GetAddressLine2(p.Addresses, AddressType.Physical), GetAddressCity(p.Addresses, AddressType.Physical), GetAddressState(p.Addresses, AddressType.Physical),
        GetAddressPostalCode(p.Addresses, AddressType.Physical), p.EmployerName, p.EmployerPhone, p.EmployerEmail, p.PrimaryProviderId, p.HipaaConsentAcknowledged,
        p.HipaaConsentDate, p.PreferredLanguage, p.Ethnicity, p.IsDeceased, p.DeceasedDate, p.DeceasedReason,
        p.InsurancePlans.Select(Map).ToList());

    private static InsuranceCarrierDto Map(InsuranceCarrier c) => new(
        c.Id, c.CarrierName, c.PayerId, GetAddressPhone(c.Addresses, AddressType.Physical, PhoneType.Primary), c.Email, c.AuthorizationPhone, c.AuthorizationEmail,
        GetAddressLine1(c.Addresses, AddressType.Physical), GetAddressLine2(c.Addresses, AddressType.Physical), GetAddressCity(c.Addresses, AddressType.Physical), GetAddressState(c.Addresses, AddressType.Physical), GetAddressPostalCode(c.Addresses, AddressType.Physical),
        c.SupportedPlans.Select(x => new SupportedPlanDto(x.Id, x.PlanCode, x.PlanName, x.IsActive)).ToList());

    private static ProviderDto Map(Provider p) => new(p.Id, p.FullName, p.Specialty, p.Npi);
    private static ProviderScheduleTemplateDto Map(ProviderScheduleTemplate t) => new(t.Id, t.ProviderId, t.PracticeLocationId, t.DayOfWeek, t.StartTime, t.EndTime, t.EffectiveStartDate, t.EffectiveEndDate, t.IsActive);
    private static TenantDto Map(Tenant t) => new(t.Id, t.Name, GetAddressLine1(t.Addresses, AddressType.Physical), GetAddressLine2(t.Addresses, AddressType.Physical), GetAddressCity(t.Addresses, AddressType.Physical), GetAddressState(t.Addresses, AddressType.Physical), GetAddressPostalCode(t.Addresses, AddressType.Physical), GetAddressLine1(t.Addresses, AddressType.Billing), GetAddressLine2(t.Addresses, AddressType.Billing), GetAddressCity(t.Addresses, AddressType.Billing), GetAddressState(t.Addresses, AddressType.Billing), GetAddressPostalCode(t.Addresses, AddressType.Billing), GetAddressPhone(t.Addresses, AddressType.Physical, PhoneType.Primary), GetAddressPhone(t.Addresses, AddressType.Billing, PhoneType.Secondary), t.PrimaryEmail, t.SecondaryEmail);
    private static PracticeLocationDto Map(PracticeLocation l) => new(l.Id, l.TenantId, l.LocationName, GetAddressLine1(l.Addresses, AddressType.Physical), GetAddressLine2(l.Addresses, AddressType.Physical), GetAddressCity(l.Addresses, AddressType.Physical), GetAddressState(l.Addresses, AddressType.Physical), GetAddressPostalCode(l.Addresses, AddressType.Physical), GetAddressLine1(l.Addresses, AddressType.Billing), GetAddressLine2(l.Addresses, AddressType.Billing), GetAddressCity(l.Addresses, AddressType.Billing), GetAddressState(l.Addresses, AddressType.Billing), GetAddressPostalCode(l.Addresses, AddressType.Billing), GetAddressPhone(l.Addresses, AddressType.Physical, PhoneType.Primary), GetAddressPhone(l.Addresses, AddressType.Billing, PhoneType.Secondary), l.PrimaryEmail, l.SecondaryEmail, l.IsActive);
    private static AppointmentDto Map(Appointment a) => new(a.Id, a.PatientId, a.ProviderId, a.PracticeLocationId, a.StartTime, a.EndTime, a.Reason, a.Status);
    private static InvoiceDto Map(Invoice i) => new(i.Id, i.PatientId, i.AppointmentId, i.TotalAmount, i.PaidAmount, i.Status);

    private static Address CreateAddress(AddressType addressType, string line1, string line2, string line3, string city, string state, string postalCode, string phone, PhoneType phoneType)
        => new(addressType, line1, line2, line3, city, state, postalCode, phone, phoneType);

    private static Address? FindAddress(IEnumerable<Address> addresses, AddressType addressType, PhoneType phoneType)
        => addresses.FirstOrDefault(x => x.AddressType == addressType && x.PhoneType == phoneType)
            ?? addresses.FirstOrDefault(x => x.AddressType == addressType);

    private static string GetAddressLine1(IEnumerable<Address> addresses, AddressType addressType) => FindAddress(addresses, addressType, PhoneType.Primary)?.AddressLine1 ?? string.Empty;
    private static string GetAddressLine2(IEnumerable<Address> addresses, AddressType addressType) => FindAddress(addresses, addressType, PhoneType.Primary)?.AddressLine2 ?? string.Empty;
    private static string GetAddressCity(IEnumerable<Address> addresses, AddressType addressType) => FindAddress(addresses, addressType, PhoneType.Primary)?.City ?? string.Empty;
    private static string GetAddressState(IEnumerable<Address> addresses, AddressType addressType) => FindAddress(addresses, addressType, PhoneType.Primary)?.State ?? string.Empty;
    private static string GetAddressPostalCode(IEnumerable<Address> addresses, AddressType addressType) => FindAddress(addresses, addressType, PhoneType.Primary)?.PostalCode ?? string.Empty;
    private static string GetAddressPhone(IEnumerable<Address> addresses, AddressType addressType, PhoneType phoneType) => FindAddress(addresses, addressType, phoneType)?.Phone ?? string.Empty;

}
