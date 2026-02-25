-- MediTransact PostgreSQL schema bootstrap
-- Mirrors current EF Core model in PracticeDbContext.

BEGIN;

CREATE TABLE IF NOT EXISTS "Providers" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "FullName" text NOT NULL,
  "Specialty" text NOT NULL,
  "Npi" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "Tenants" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "Name" text NOT NULL,
  "PrimaryEmail" text NOT NULL,
  "SecondaryEmail" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "PracticeLocations" (
  "Id" uuid PRIMARY KEY,
  "TenantId" uuid NOT NULL,
  "LocationName" text NOT NULL,
  "PrimaryEmail" text NOT NULL,
  "SecondaryEmail" text NOT NULL,
  "IsActive" boolean NOT NULL,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  CONSTRAINT "FK_PracticeLocations_Tenants_TenantId"
    FOREIGN KEY ("TenantId") REFERENCES "Tenants"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Patients" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "FirstName" text NOT NULL,
  "MiddleName" text NOT NULL,
  "LastName" text NOT NULL,
  "DateOfBirth" date NOT NULL,
  "Sex" text NOT NULL,
  "IdentificationType" text NOT NULL,
  "IdentificationNumber" text NOT NULL,
  "MaritalStatus" text NOT NULL,
  "PreferredLanguage" text NOT NULL,
  "Ethnicity" text NOT NULL,
  "PatientEmail" text NOT NULL,
  "EmployerName" text NOT NULL,
  "EmployerPhone" text NOT NULL,
  "EmployerEmail" text NOT NULL,
  "PrimaryProviderId" uuid NULL,
  "HipaaConsentAcknowledged" boolean NOT NULL,
  "HipaaConsentDate" date NULL,
  "IsDeceased" boolean NOT NULL,
  "DeceasedDate" date NULL,
  "DeceasedReason" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "InsuranceCarriers" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "CarrierName" text NOT NULL,
  "PayerId" text NOT NULL,
  "Email" text NOT NULL,
  "AuthorizationPhone" text NOT NULL,
  "AuthorizationEmail" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "Appointments" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "PatientId" uuid NOT NULL,
  "ProviderId" uuid NOT NULL,
  "PracticeLocationId" uuid NOT NULL,
  "StartTime" timestamp with time zone NOT NULL,
  "EndTime" timestamp with time zone NOT NULL,
  "Reason" text NOT NULL,
  "Status" integer NOT NULL,
  CONSTRAINT "FK_Appointments_PracticeLocations_PracticeLocationId"
    FOREIGN KEY ("PracticeLocationId") REFERENCES "PracticeLocations"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Invoices" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "PatientId" uuid NOT NULL,
  "AppointmentId" uuid NULL,
  "TotalAmount" numeric(18,2) NOT NULL,
  "Status" integer NOT NULL,
  "PaidAmount" numeric(18,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS "ChargeCodeTypes" (
  "Id" uuid PRIMARY KEY,
  "Name" text NOT NULL,
  "IsActive" boolean NOT NULL,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS "PatientCases" (
  "Id" uuid PRIMARY KEY,
  "PatientId" uuid NOT NULL,
  "CaseNumber" text NOT NULL,
  "CaseName" text NOT NULL,
  "Description" text NOT NULL,
  "OpenedDate" date NOT NULL,
  "ClosedDate" date NULL,
  "IsActive" boolean NOT NULL,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  CONSTRAINT "FK_PatientCases_Patients_PatientId"
    FOREIGN KEY ("PatientId") REFERENCES "Patients"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CaseCharges" (
  "Id" uuid PRIMARY KEY,
  "PatientCaseId" uuid NOT NULL,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "DateOfService" date NOT NULL,
  "DateOfEntry" timestamp with time zone NOT NULL,
  "DateOfBilling" date NULL,
  "ClaimType" integer NOT NULL,
  "InsuranceCarrierId" uuid NOT NULL,
  "InsurancePlanCode" text NOT NULL,
  "BillingStatus" integer NOT NULL,
  CONSTRAINT "FK_CaseCharges_PatientCases_PatientCaseId"
    FOREIGN KEY ("PatientCaseId") REFERENCES "PatientCases"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CaseChargeCodes" (
  "Id" uuid PRIMARY KEY,
  "CaseChargeId" uuid NOT NULL,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "ChargeCodeTypeId" uuid NOT NULL,
  "Code" text NOT NULL,
  "Units" numeric NOT NULL,
  CONSTRAINT "FK_CaseChargeCodes_CaseCharges_CaseChargeId"
    FOREIGN KEY ("CaseChargeId") REFERENCES "CaseCharges"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "SupportedInsurancePlans" (
  "Id" uuid PRIMARY KEY,
  "InsuranceCarrierId" uuid NOT NULL,
  "PlanCode" text NOT NULL,
  "PlanName" text NOT NULL,
  "IsActive" boolean NOT NULL,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  CONSTRAINT "FK_SupportedInsurancePlans_InsuranceCarriers_InsuranceCarrierId"
    FOREIGN KEY ("InsuranceCarrierId") REFERENCES "InsuranceCarriers"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "PatientInsurancePlans" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "PatientId" uuid NOT NULL,
  "InsuranceCarrierId" uuid NOT NULL,
  "PlanCode" text NOT NULL,
  "GroupNumber" text NOT NULL,
  "MemberNumber" text NOT NULL,
  "AuthorizationContactName" text NOT NULL,
  "AuthorizationPhone" text NOT NULL,
  "AuthorizationEmail" text NOT NULL,
  "AuthorizationStartDate" date NOT NULL,
  "AuthorizationEndDate" date NULL,
  CONSTRAINT "FK_PatientInsurancePlans_Patients_PatientId"
    FOREIGN KEY ("PatientId") REFERENCES "Patients"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Addresses" (
  "Id" uuid PRIMARY KEY,
  "IsActive" boolean NOT NULL DEFAULT TRUE,
  "IsDeleted" boolean NOT NULL DEFAULT FALSE,
  "AddressType" integer NOT NULL,
  "AddressLine1" text NOT NULL,
  "AddressLine2" text NOT NULL,
  "AddressLine3" text NOT NULL,
  "City" text NOT NULL,
  "State" text NOT NULL,
  "PostalCode" text NOT NULL,
  "Phone" text NOT NULL,
  "PhoneType" integer NOT NULL,
  "TenantId" uuid NULL,
  "PracticeLocationId" uuid NULL,
  "InsuranceCarrierId" uuid NULL,
  "PatientId" uuid NULL,
  CONSTRAINT "FK_Addresses_Tenants_TenantId"
    FOREIGN KEY ("TenantId") REFERENCES "Tenants"("Id") ON DELETE CASCADE,
  CONSTRAINT "FK_Addresses_PracticeLocations_PracticeLocationId"
    FOREIGN KEY ("PracticeLocationId") REFERENCES "PracticeLocations"("Id") ON DELETE CASCADE,
  CONSTRAINT "FK_Addresses_InsuranceCarriers_InsuranceCarrierId"
    FOREIGN KEY ("InsuranceCarrierId") REFERENCES "InsuranceCarriers"("Id") ON DELETE CASCADE,
  CONSTRAINT "FK_Addresses_Patients_PatientId"
    FOREIGN KEY ("PatientId") REFERENCES "Patients"("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Providers_Npi" ON "Providers" ("Npi");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Tenants_Name" ON "Tenants" ("Name");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PracticeLocations_TenantId_LocationName" ON "PracticeLocations" ("TenantId", "LocationName");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_ChargeCodeTypes_Name" ON "ChargeCodeTypes" ("Name");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_SupportedInsurancePlans_InsuranceCarrierId_PlanCode" ON "SupportedInsurancePlans" ("InsuranceCarrierId", "PlanCode");

CREATE INDEX IF NOT EXISTS "IX_Addresses_AddressType_PhoneType_PostalCode" ON "Addresses" ("AddressType", "PhoneType", "PostalCode");
CREATE INDEX IF NOT EXISTS "IX_PatientInsurancePlans_PatientId_InsuranceCarrierId_MemberNumber" ON "PatientInsurancePlans" ("PatientId", "InsuranceCarrierId", "MemberNumber");
CREATE INDEX IF NOT EXISTS "IX_CaseChargeCodes_CaseChargeId_ChargeCodeTypeId_Code" ON "CaseChargeCodes" ("CaseChargeId", "ChargeCodeTypeId", "Code");

COMMIT;
