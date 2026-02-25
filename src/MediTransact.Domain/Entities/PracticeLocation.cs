namespace MediTransact.Domain.Entities;

public class PracticeLocation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public string LocationName { get; private set; }
    public string PrimaryEmail { get; private set; }
    public string SecondaryEmail { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }

    public List<Address> Addresses { get; private set; } = [];

    private PracticeLocation()
    {
        LocationName = PrimaryEmail = SecondaryEmail = string.Empty;
    }

    public PracticeLocation(Guid tenantId, string locationName, string primaryEmail, string secondaryEmail, bool isActive, IEnumerable<Address> addresses)
    {
        TenantId = tenantId;
        LocationName = locationName;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        IsActive = isActive;
        IsDeleted = false;
        Addresses = addresses.ToList();
    }

    public void Update(string locationName, string primaryEmail, string secondaryEmail, bool isActive, IEnumerable<Address> addresses)
    {
        LocationName = locationName;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        IsActive = isActive;
        IsDeleted = false;
        Addresses = addresses.ToList();
    }
}
