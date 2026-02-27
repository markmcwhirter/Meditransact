namespace MediTransact.Domain.Entities;

public class PatientInsurancePlan
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public Guid PatientId { get; private set; }
    public Guid InsuranceCarrierId { get; private set; }
    public string PlanCode { get; private set; }
    public string GroupNumber { get; private set; }
    public string MemberNumber { get; private set; }
    public string AuthorizationContactName { get; private set; }
    public string AuthorizationPhone { get; private set; }
    public string AuthorizationEmail { get; private set; }
    public DateOnly AuthorizationStartDate { get; private set; }
    public DateOnly? AuthorizationEndDate { get; private set; }

    private PatientInsurancePlan()
    {
        PlanCode = GroupNumber = MemberNumber = AuthorizationContactName = AuthorizationPhone = AuthorizationEmail = string.Empty;
    }

    public PatientInsurancePlan(
        Guid insuranceCarrierId,
        string planCode,
        string groupNumber,
        string memberNumber,
        string authorizationContactName,
        string authorizationPhone,
        string authorizationEmail,
        DateOnly authorizationStartDate,
        DateOnly? authorizationEndDate)
    {
        InsuranceCarrierId = insuranceCarrierId;
        PlanCode = planCode;
        GroupNumber = groupNumber;
        MemberNumber = memberNumber;
        AuthorizationContactName = authorizationContactName;
        AuthorizationPhone = authorizationPhone;
        AuthorizationEmail = authorizationEmail;
        AuthorizationStartDate = authorizationStartDate;
        AuthorizationEndDate = authorizationEndDate;
    }
}
