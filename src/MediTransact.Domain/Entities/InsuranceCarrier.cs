namespace MediTransact.Domain.Entities;

public class InsuranceCarrier
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public string CarrierName { get; private set; }
    public string PayerId { get; private set; }
    public string Email { get; private set; }
    public string AuthorizationPhone { get; private set; }
    public string AuthorizationEmail { get; private set; }

    public List<Address> Addresses { get; private set; } = [];
    public List<SupportedInsurancePlan> SupportedPlans { get; private set; } = [];

    private InsuranceCarrier()
    {
        CarrierName = PayerId = Email = AuthorizationPhone = AuthorizationEmail = string.Empty;
    }

    public InsuranceCarrier(
        string carrierName,
        string payerId,
        string email,
        string authorizationPhone,
        string authorizationEmail,
        IEnumerable<Address> addresses,
        IEnumerable<SupportedInsurancePlan> supportedPlans)
    {
        CarrierName = carrierName;
        PayerId = payerId;
        Email = email;
        AuthorizationPhone = authorizationPhone;
        AuthorizationEmail = authorizationEmail;
        Addresses = addresses.ToList();
        SupportedPlans = supportedPlans.ToList();
    }

    public void Update(
        string carrierName,
        string payerId,
        string email,
        string authorizationPhone,
        string authorizationEmail,
        IEnumerable<Address> addresses,
        IEnumerable<SupportedInsurancePlan> supportedPlans)
    {
        CarrierName = carrierName;
        PayerId = payerId;
        Email = email;
        AuthorizationPhone = authorizationPhone;
        AuthorizationEmail = authorizationEmail;
        Addresses = addresses.ToList();
        SupportedPlans = supportedPlans.ToList();
    }
}

public class SupportedInsurancePlan
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid InsuranceCarrierId { get; private set; }
    public string PlanCode { get; private set; }
    public string PlanName { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    private SupportedInsurancePlan()
    {
        PlanCode = PlanName = string.Empty;
    }

    public SupportedInsurancePlan(string planCode, string planName, bool isActive)
    {
        PlanCode = planCode;
        PlanName = planName;
        IsActive = isActive;
    }
}
