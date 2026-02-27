namespace MediTransact.Domain.Entities;

public enum ClaimType
{
    Professional,
    Institutional,
    Dental,
    Vision,
    Pharmacy,
    WorkersComp
}

public enum BillingStatus
{
    Draft,
    ReadyToBill,
    Submitted,
    Denied,
    Paid,
    Voided
}

public class PatientCase
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PatientId { get; private set; }
    public string CaseNumber { get; private set; }
    public string CaseName { get; private set; }
    public string Description { get; private set; }
    public DateOnly OpenedDate { get; private set; }
    public DateOnly? ClosedDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }

    public List<CaseCharge> Charges { get; private set; } = [];

    private PatientCase()
    {
        CaseNumber = CaseName = Description = string.Empty;
    }

    public PatientCase(Guid patientId, string caseNumber, string caseName, string description, DateOnly openedDate, DateOnly? closedDate, bool isActive)
    {
        PatientId = patientId;
        CaseNumber = caseNumber;
        CaseName = caseName;
        Description = description;
        OpenedDate = openedDate;
        ClosedDate = closedDate;
        IsActive = isActive;
        IsDeleted = false;
    }

    public void Update(string caseName, string description, DateOnly openedDate, DateOnly? closedDate, bool isActive)
    {
        CaseName = caseName;
        Description = description;
        OpenedDate = openedDate;
        ClosedDate = closedDate;
        IsActive = isActive;
        IsDeleted = false;
    }
}

public class CaseCharge
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PatientCaseId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public DateOnly DateOfService { get; private set; }
    public DateTimeOffset DateOfEntry { get; private set; }
    public DateOnly? DateOfBilling { get; private set; }
    public ClaimType ClaimType { get; private set; }
    public Guid InsuranceCarrierId { get; private set; }
    public string InsurancePlanCode { get; private set; }
    public BillingStatus BillingStatus { get; private set; }

    public List<CaseChargeCode> ChargeCodes { get; private set; } = [];

    private CaseCharge()
    {
        InsurancePlanCode = string.Empty;
    }

    public CaseCharge(
        DateOnly dateOfService,
        DateTimeOffset dateOfEntry,
        DateOnly? dateOfBilling,
        ClaimType claimType,
        Guid insuranceCarrierId,
        string insurancePlanCode,
        BillingStatus billingStatus,
        IEnumerable<CaseChargeCode> chargeCodes)
    {
        DateOfService = dateOfService;
        DateOfEntry = dateOfEntry;
        DateOfBilling = dateOfBilling;
        ClaimType = claimType;
        InsuranceCarrierId = insuranceCarrierId;
        InsurancePlanCode = insurancePlanCode;
        BillingStatus = billingStatus;
        ChargeCodes = chargeCodes.ToList();
    }

    public void Update(
        DateOnly dateOfService,
        DateTimeOffset dateOfEntry,
        DateOnly? dateOfBilling,
        ClaimType claimType,
        Guid insuranceCarrierId,
        string insurancePlanCode,
        BillingStatus billingStatus,
        IEnumerable<CaseChargeCode> chargeCodes)
    {
        DateOfService = dateOfService;
        DateOfEntry = dateOfEntry;
        DateOfBilling = dateOfBilling;
        ClaimType = claimType;
        InsuranceCarrierId = insuranceCarrierId;
        InsurancePlanCode = insurancePlanCode;
        BillingStatus = billingStatus;
        ChargeCodes = chargeCodes.ToList();
    }
}

public class CaseChargeCode
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CaseChargeId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public Guid ChargeCodeTypeId { get; private set; }
    public string Code { get; private set; }
    public decimal Units { get; private set; }

    private CaseChargeCode()
    {
        Code = string.Empty;
    }

    public CaseChargeCode(Guid chargeCodeTypeId, string code, decimal units)
    {
        ChargeCodeTypeId = chargeCodeTypeId;
        Code = code;
        Units = units;
    }
}

public class ChargeCodeType
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }

    private ChargeCodeType()
    {
        Name = string.Empty;
    }

    public ChargeCodeType(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
        IsDeleted = false;
    }

    public void Update(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
        IsDeleted = false;
    }
}
