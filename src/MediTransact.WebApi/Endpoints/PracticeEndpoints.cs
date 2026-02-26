using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using MediTransact.Application.DTOs;
using MediTransact.Application.Errors;
using MediTransact.Application.Interfaces;
using MediTransact.WebApi.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace MediTransact.WebApi.Endpoints;


file static class AuthRoles
{
    public const string Authenticated = "authenticated";
    public const string SuperUser = "super user";
    public const string TenantAdministrator = "tenant administrator";
    public const string LocationAdministrator = "location administrator";
    public const string LocationScheduler = "location scheduler";
    public const string LocationSchedulerReadonly = "location scheduler readonly";
    public const string LocationBilling = "location billing";
    public const string LocationBillingReadonly = "location billing readonly";
    public const string LocationRecords = "location records";
    public const string LocationRecordsReadonly = "location records readonly";
    public const string LocationReporting = "location reporting";

    public static readonly string[] AllRoles =
    [
        Authenticated,
        SuperUser,
        TenantAdministrator,
        LocationAdministrator,
        LocationScheduler,
        LocationSchedulerReadonly,
        LocationBilling,
        LocationBillingReadonly,
        LocationRecords,
        LocationRecordsReadonly,
        LocationReporting
    ];
}

file static class EndpointErrorMapper
{
    public static int ToStatusCode(this AppError error) => error.Kind switch
    {
        ErrorKind.NotFound => 404,
        ErrorKind.Validation => 400,
        ErrorKind.Conflict => 409,
        ErrorKind.Unauthorized => 401,
        _ => 500
    };
}

public abstract class PracticeEndpoint<TRequest>(IPracticeService service) : Endpoint<TRequest, object> where TRequest : notnull
{
    protected readonly IPracticeService Service = service;

    protected async Task SendResultAsync<T>(OneOf.OneOf<T, AppError> result, CancellationToken ct)
    {
        if (result.IsT0) await SendAsync(result.AsT0!, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }

    protected async Task SendSuccessResultAsync(OneOf.OneOf<OneOf.Types.Success, AppError> result, CancellationToken ct)
    {
        if (result.IsT0) await SendNoContentAsync(ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public abstract class PracticeEndpointWithoutRequest(IPracticeService service) : EndpointWithoutRequest<object>
{
    protected readonly IPracticeService Service = service;

    protected async Task SendResultAsync<T>(OneOf.OneOf<T, AppError> result, CancellationToken ct)
    {
        if (result.IsT0) await SendAsync(result.AsT0!, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class LoginEndpoint(IConfiguration configuration) : Endpoint<LoginRequest, object>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        var users = configuration.GetSection("Auth:Users").Get<List<AuthUser>>() ?? [];
        var user = users.FirstOrDefault(u =>
            string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase)
            && u.Password == request.Password);

        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var jwt = configuration.GetSection("Jwt");
        var issuer = jwt["Issuer"] ?? "MediTransact";
        var audience = jwt["Audience"] ?? "MediTransactClient";
        var key = jwt["Key"] ?? "ReplaceWithLongDevelopmentKey_AtLeast32Characters";
        var expiresInSeconds = int.TryParse(jwt["ExpiresInSeconds"], out var exp) ? exp : 3600;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, AuthRoles.Authenticated)
        };

        if (!string.IsNullOrWhiteSpace(user.Role))
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Trim()));

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(expiresInSeconds),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256));

        await SendAsync(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresInSeconds), 200, ct);
    }

    private sealed class AuthUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = AuthRoles.Authenticated;
    }
}

public class RegisterPatientEndpoint(IPracticeService service) : PracticeEndpoint<RegisterPatientRequest>(service)
{
    public override void Configure() { Post("/api/practice/patients"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(RegisterPatientRequest req, CancellationToken ct) => await SendResultAsync(await Service.RegisterPatientAsync(req, ct), ct);
}

public class ListPatientsEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{
    public override void Configure() { Get("/api/practice/patients"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListPatientsAsync(ct), ct);
}

public class GetPatientEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{
    public override void Configure() { Get("/api/practice/patients/{id:guid}"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await service.GetPatientByIdAsync(id, ct);
        if (result.IsT0) await SendAsync(result.AsT0, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class UpdatePatientEndpoint(IPracticeService service) : Endpoint<UpdatePatientRequest>
{
    public override void Configure() { Put("/api/practice/patients/{id:guid}"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(UpdatePatientRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await service.UpdatePatientAsync(id, req, ct);
        if (result.IsT0) await SendNoContentAsync(ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class DeletePatientEndpoint(IPracticeService service) : EndpointWithoutRequest
{
    public override void Configure() { Delete("/api/practice/patients/{id:guid}"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await service.DeletePatientAsync(Route<Guid>("id"), ct);
        if (result.IsT0) await SendNoContentAsync(ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class RegisterProviderEndpoint(IPracticeService service) : PracticeEndpoint<RegisterProviderRequest>(service)
{ public override void Configure() { Post("/api/practice/providers"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(RegisterProviderRequest req, CancellationToken ct) => await SendResultAsync(await Service.RegisterProviderAsync(req, ct), ct); }
public class ListProvidersEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/providers"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListProvidersAsync(ct), ct); }
public class GetProviderEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/providers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetProviderByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateProviderEndpoint(IPracticeService service) : Endpoint<UpdateProviderRequest>
{ public override void Configure() { Put("/api/practice/providers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateProviderRequest req, CancellationToken ct){var r=await service.UpdateProviderAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteProviderEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/providers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteProviderAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }


public class CreateProviderScheduleTemplateEndpoint(IPracticeService service) : Endpoint<CreateProviderScheduleTemplateRequest>
{
    public override void Configure() { Post("/api/practice/providers/{providerId:guid}/schedule-templates"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CreateProviderScheduleTemplateRequest req, CancellationToken ct)
    {
        var r = await service.CreateProviderScheduleTemplateAsync(Route<Guid>("providerId"), req, ct);
        if (r.IsT0) await SendAsync(r.AsT0, 200, ct); else await SendAsync(r.AsT1, r.AsT1.ToStatusCode(), ct);
    }
}

public class ListProviderScheduleTemplatesEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{
    public override void Configure() { Get("/api/practice/providers/{providerId:guid}/schedule-templates"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var r = await service.ListProviderScheduleTemplatesAsync(Route<Guid>("providerId"), ct);
        if (r.IsT0) await SendAsync(r.AsT0, 200, ct); else await SendAsync(r.AsT1, r.AsT1.ToStatusCode(), ct);
    }
}

public class GetProviderScheduleTemplateEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/schedule-templates/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetProviderScheduleTemplateByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateProviderScheduleTemplateEndpoint(IPracticeService service) : Endpoint<UpdateProviderScheduleTemplateRequest>
{ public override void Configure() { Put("/api/practice/schedule-templates/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateProviderScheduleTemplateRequest req, CancellationToken ct){var r=await service.UpdateProviderScheduleTemplateAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteProviderScheduleTemplateEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/schedule-templates/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteProviderScheduleTemplateAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class ScheduleAppointmentEndpoint(IPracticeService service) : PracticeEndpoint<ScheduleAppointmentRequest>(service)
{ public override void Configure() { Post("/api/practice/appointments"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(ScheduleAppointmentRequest req, CancellationToken ct) => await SendResultAsync(await Service.ScheduleAppointmentAsync(req, ct), ct); }
public class ListAppointmentsEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/appointments"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListAppointmentsAsync(ct), ct); }
public class GetAppointmentEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/appointments/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetAppointmentByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateAppointmentEndpoint(IPracticeService service) : Endpoint<UpdateAppointmentRequest>
{ public override void Configure() { Put("/api/practice/appointments/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateAppointmentRequest req, CancellationToken ct){var r=await service.UpdateAppointmentAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteAppointmentEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/appointments/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteAppointmentAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreateInvoiceEndpoint(IPracticeService service) : PracticeEndpoint<CreateInvoiceRequest>(service)
{ public override void Configure() { Post("/api/practice/invoices"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CreateInvoiceRequest req, CancellationToken ct) => await SendResultAsync(await Service.CreateInvoiceAsync(req, ct), ct); }
public class ListInvoicesEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/invoices"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListInvoicesAsync(ct), ct); }
public class GetInvoiceEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/invoices/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetInvoiceByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateInvoiceEndpoint(IPracticeService service) : Endpoint<UpdateInvoiceRequest>
{ public override void Configure() { Put("/api/practice/invoices/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateInvoiceRequest req, CancellationToken ct){var r=await service.UpdateInvoiceAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteInvoiceEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/invoices/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteInvoiceAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class RecordPaymentEndpoint(IPracticeService service) : Endpoint<RecordPaymentRequest>
{ public override void Configure() { Post("/api/practice/payments"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(RecordPaymentRequest req, CancellationToken ct){var r=await service.RecordPaymentAsync(req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DashboardEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/dashboard"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.GetDashboardAsync(ct), ct); }


public class CreateTenantEndpoint(IPracticeService service) : PracticeEndpoint<RegisterTenantRequest>(service)
{
    public override void Configure() { Post("/api/practice/tenants"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(RegisterTenantRequest req, CancellationToken ct) => await SendResultAsync(await Service.CreateTenantAsync(req, ct), ct);
}

public class ListTenantsEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{
    public override void Configure() { Get("/api/practice/tenants"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListTenantsAsync(ct), ct);
}

public class GetTenantEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/tenants/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetTenantByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateTenantEndpoint(IPracticeService service) : Endpoint<UpdateTenantRequest>
{ public override void Configure() { Put("/api/practice/tenants/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateTenantRequest req, CancellationToken ct){var r=await service.UpdateTenantAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteTenantEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/tenants/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteTenantAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreatePracticeLocationEndpoint(IPracticeService service) : Endpoint<RegisterPracticeLocationRequest>
{
    public override void Configure() { Post("/api/practice/tenants/{tenantId:guid}/locations"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(RegisterPracticeLocationRequest req, CancellationToken ct)
    {
        var result = await service.CreatePracticeLocationAsync(Route<Guid>("tenantId"), req, ct);
        if (result.IsT0) await SendAsync(result.AsT0, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class ListPracticeLocationsEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{
    public override void Configure() { Get("/api/practice/tenants/{tenantId:guid}/locations"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await service.ListPracticeLocationsAsync(Route<Guid>("tenantId"), ct);
        if (result.IsT0) await SendAsync(result.AsT0, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class GetPracticeLocationEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/locations/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetPracticeLocationByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdatePracticeLocationEndpoint(IPracticeService service) : Endpoint<UpdatePracticeLocationRequest>
{ public override void Configure() { Put("/api/practice/locations/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdatePracticeLocationRequest req, CancellationToken ct){var r=await service.UpdatePracticeLocationAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeletePracticeLocationEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/locations/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeletePracticeLocationAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreateInsuranceCarrierEndpoint(IPracticeService service) : PracticeEndpoint<RegisterInsuranceCarrierRequest>(service)
{ public override void Configure() { Post("/api/practice/insurance-carriers"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(RegisterInsuranceCarrierRequest req, CancellationToken ct) => await SendResultAsync(await Service.CreateInsuranceCarrierAsync(req, ct), ct); }

public class ListInsuranceCarriersEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/insurance-carriers"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListInsuranceCarriersAsync(ct), ct); }

public class GetInsuranceCarrierEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/insurance-carriers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetInsuranceCarrierByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class UpdateInsuranceCarrierEndpoint(IPracticeService service) : Endpoint<UpdateInsuranceCarrierRequest>
{ public override void Configure() { Put("/api/practice/insurance-carriers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateInsuranceCarrierRequest req, CancellationToken ct){var r=await service.UpdateInsuranceCarrierAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class DeleteInsuranceCarrierEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/insurance-carriers/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteInsuranceCarrierAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreatePatientCaseEndpoint(IPracticeService service) : Endpoint<PatientCaseInput>
{
    public override void Configure() { Post("/api/practice/patients/{patientId:guid}/cases"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(PatientCaseInput req, CancellationToken ct)
    {
        var result = await service.CreatePatientCaseAsync(Route<Guid>("patientId"), req, ct);
        if (result.IsT0) await SendAsync(result.AsT0, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class ListPatientCasesEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{
    public override void Configure() { Get("/api/practice/patients/{patientId:guid}/cases"); Roles(AuthRoles.AllRoles); }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await service.ListPatientCasesAsync(Route<Guid>("patientId"), ct);
        if (result.IsT0) await SendAsync(result.AsT0, 200, ct);
        else await SendAsync(result.AsT1, result.AsT1.ToStatusCode(), ct);
    }
}

public class GetPatientCaseEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/cases/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetPatientCaseByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdatePatientCaseEndpoint(IPracticeService service) : Endpoint<UpdatePatientCaseRequest>
{ public override void Configure() { Put("/api/practice/cases/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdatePatientCaseRequest req, CancellationToken ct){var r=await service.UpdatePatientCaseAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeletePatientCaseEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/cases/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeletePatientCaseAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreateCaseChargeEndpoint(IPracticeService service) : Endpoint<CaseChargeInput>
{ public override void Configure() { Post("/api/practice/cases/{caseId:guid}/charges"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CaseChargeInput req, CancellationToken ct){var r=await service.CreateCaseChargeAsync(Route<Guid>("caseId"),req,ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class ListCaseChargesEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/cases/{caseId:guid}/charges"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.ListCaseChargesAsync(Route<Guid>("caseId"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class GetCaseChargeEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/charges/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetCaseChargeByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateCaseChargeEndpoint(IPracticeService service) : Endpoint<UpdateCaseChargeRequest>
{ public override void Configure() { Put("/api/practice/charges/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(UpdateCaseChargeRequest req, CancellationToken ct){var r=await service.UpdateCaseChargeAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteCaseChargeEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/charges/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteCaseChargeAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }

public class CreateChargeCodeTypeEndpoint(IPracticeService service) : Endpoint<ChargeCodeTypeRequest>
{ public override void Configure() { Post("/api/practice/charge-code-types"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(ChargeCodeTypeRequest req, CancellationToken ct){var r=await service.CreateChargeCodeTypeAsync(req,ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class ListChargeCodeTypesEndpoint(IPracticeService service) : PracticeEndpointWithoutRequest(service)
{ public override void Configure() { Get("/api/practice/charge-code-types"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct) => await SendResultAsync(await Service.ListChargeCodeTypesAsync(ct), ct); }
public class GetChargeCodeTypeEndpoint(IPracticeService service) : EndpointWithoutRequest<object>
{ public override void Configure() { Get("/api/practice/charge-code-types/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.GetChargeCodeTypeByIdAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendAsync(r.AsT0,200,ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class UpdateChargeCodeTypeEndpoint(IPracticeService service) : Endpoint<ChargeCodeTypeRequest>
{ public override void Configure() { Put("/api/practice/charge-code-types/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(ChargeCodeTypeRequest req, CancellationToken ct){var r=await service.UpdateChargeCodeTypeAsync(Route<Guid>("id"),req,ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
public class DeleteChargeCodeTypeEndpoint(IPracticeService service) : EndpointWithoutRequest
{ public override void Configure() { Delete("/api/practice/charge-code-types/{id:guid}"); Roles(AuthRoles.AllRoles); } public override async Task HandleAsync(CancellationToken ct){var r=await service.DeleteChargeCodeTypeAsync(Route<Guid>("id"),ct); if(r.IsT0) await SendNoContentAsync(ct); else await SendAsync(r.AsT1,r.AsT1.ToStatusCode(),ct);} }
