# MediTransact (Clean + DDD)

MediTransact is now a **web application** with:

- **C# Web API** (running on **C# 14** in production via `LangVersion=14.0`) targeting **.NET 10.0**
- **Entity Framework Core** persistence with **PostgreSQL**
- **JWT Authentication + Authorization** for secured APIs
- **React v21** frontend shell
- **Clean Architecture** layering (`Domain`, `Application`, `Infrastructure`, `WebApi`)
- **DDD-style domain model** for core healthcare office operations
- **Unit tests** using **Moq** + **xUnit v3**
- **OneOf-based result handling** with typed `AppError` payloads (no exception-based control flow for CRUD operations)
- **FluentValidation** request validation on API endpoints

## Solution structure

- `src/MediTransact.Domain` — Entities and repository contracts
- `src/MediTransact.Application` — Use-case contracts/services (application layer)
- `src/MediTransact.Infrastructure` — EF Core DbContext factory (`IDbContextFactory`) + repository/read model
- `src/MediTransact.WebApi` — FastEndpoints HTTP endpoints + DI bootstrap + JWT auth
- `src/MediTransact.Database` — PostgreSQL DDL scripts (`CREATE TABLE` bootstrap)
- `tests/MediTransact.UnitTests` — Application-layer unit tests with Moq/xUnit v3
- `frontend` — React v21 app shell

## Authentication & authorization requirements

1. Get a token from `POST /api/auth/login` with JSON body:

```json
{ "username": "superadmin", "password": "Pass@123" }
```

2. Use returned token in protected API calls:

```http
Authorization: Bearer <access_token>
```

3. All `/api/practice/*` endpoints require a valid JWT.


## Error model

Administrative CRUD/service operations return `OneOf<TSuccess, AppError>` (or `OneOf<Success, AppError>`), where `AppError` includes:

- `Kind` (`NotFound`, `Validation`, `Conflict`, `Unauthorized`, `Unexpected`)
- `Code` (machine-friendly code, e.g., `patient.not_found`)
- `Message` (human-readable description)

The API maps these to HTTP responses without `try/catch` flow in controllers.


## PostgreSQL schema scripts

A standalone database project now contains raw PostgreSQL `CREATE TABLE` statements:

- `src/MediTransact.Database/Scripts/001_create_tables.sql`

Apply with:

```bash
psql "$CONNECTION_STRING" -f src/MediTransact.Database/Scripts/001_create_tables.sql
```

## PostgreSQL requirement

Set the `ConnectionStrings:Default` value to a PostgreSQL connection string, for example:

```text
Host=localhost;Port=5432;Database=meditransact;Username=postgres;Password=postgres
```


## Patient information model

Patient profiles now include administrative and clinical-registration fields for:

- Primary information (name, date of birth, sex, identification type/number)
- Marital status
- Patient and employer contact information
- Primary provider assignment
- HIPAA acknowledgment tracking
- Language and ethnicity
- Insurance carriers and supported plans
- Multiple patient insurance plans (group/member numbers + authorization contact + authorization begin/end dates)
- Deceased tracking (status, date, reason)


## Cases, charges, and charge codes

The platform now supports:

- Multiple **cases per patient**
- Multiple **charges per case**
- Multiple **charge codes per charge** with units
- Multiple **locations per practice tenant**
- Practitioner schedule templates for location/time windows
- Patient scheduling at a location with practitioner, start time, and end time
- Tenant/location/patient/carrier address data normalized to a common address table (`Address`) with address type and phone type metadata
- Charge dates: date of service, date of entry, date of billing
- Claim metadata per charge: claim type, insurance plan, billing status
- Configurable charge code type catalog with supported values: **CPT**, **HCPCS**, **ICD9**, **ICD10**, **SNOMED**


## Authorization roles

The API defines and enforces these JWT role values at endpoints:

- `authenticated`
- `super user`
- `tenant administrator`
- `location administrator`
- `location scheduler`
- `location scheduler readonly`
- `location billing`
- `location billing readonly`
- `location records`
- `location records readonly`
- `location reporting`

## Administrative CRUD endpoints

### Patients
- `POST /api/practice/patients`
- `GET /api/practice/patients`
- `GET /api/practice/patients/{id}`
- `PUT /api/practice/patients/{id}`
- `DELETE /api/practice/patients/{id}`

### Providers
- `POST /api/practice/providers`
- `GET /api/practice/providers`
- `GET /api/practice/providers/{id}`
- `PUT /api/practice/providers/{id}`
- `DELETE /api/practice/providers/{id}`

### Appointments
- `POST /api/practice/appointments`
- `GET /api/practice/appointments`
- `GET /api/practice/appointments/{id}`
- `PUT /api/practice/appointments/{id}`
- `DELETE /api/practice/appointments/{id}`

### Practitioner Schedule Templates
- `POST /api/practice/providers/{providerId}/schedule-templates`
- `GET /api/practice/providers/{providerId}/schedule-templates`
- `GET /api/practice/schedule-templates/{id}`
- `PUT /api/practice/schedule-templates/{id}`
- `DELETE /api/practice/schedule-templates/{id}`

### Invoices
- `POST /api/practice/invoices`
- `GET /api/practice/invoices`
- `GET /api/practice/invoices/{id}`
- `PUT /api/practice/invoices/{id}`
- `DELETE /api/practice/invoices/{id}`

### Insurance Carriers
- `POST /api/practice/insurance-carriers`
- `GET /api/practice/insurance-carriers`
- `GET /api/practice/insurance-carriers/{id}`
- `PUT /api/practice/insurance-carriers/{id}`
- `DELETE /api/practice/insurance-carriers/{id}`


### Tenants
- `POST /api/practice/tenants`
- `GET /api/practice/tenants`
- `GET /api/practice/tenants/{id}`
- `PUT /api/practice/tenants/{id}`
- `DELETE /api/practice/tenants/{id}`


### Practice Locations
- `POST /api/practice/tenants/{tenantId}/locations`
- `GET /api/practice/tenants/{tenantId}/locations`
- `GET /api/practice/locations/{id}`
- `PUT /api/practice/locations/{id}`
- `DELETE /api/practice/locations/{id}`

### Cases
- `POST /api/practice/patients/{patientId}/cases`
- `GET /api/practice/patients/{patientId}/cases`
- `GET /api/practice/cases/{id}`
- `PUT /api/practice/cases/{id}`
- `DELETE /api/practice/cases/{id}`

### Charges
- `POST /api/practice/cases/{caseId}/charges`
- `GET /api/practice/cases/{caseId}/charges`
- `GET /api/practice/charges/{id}`
- `PUT /api/practice/charges/{id}`
- `DELETE /api/practice/charges/{id}`

### Charge Code Types
- `POST /api/practice/charge-code-types`
- `GET /api/practice/charge-code-types`
- `GET /api/practice/charge-code-types/{id}`
- `PUT /api/practice/charge-code-types/{id}`
- `DELETE /api/practice/charge-code-types/{id}`

### Other
- `POST /api/practice/payments`
- `GET /api/practice/dashboard`

## Notes

The current environment did not include the .NET SDK, so build/test commands are provided but could not be executed here.
