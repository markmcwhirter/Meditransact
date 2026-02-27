namespace MediTransact.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public string Name { get; private set; }
    public string PrimaryEmail { get; private set; }
    public string SecondaryEmail { get; private set; }

    public List<Address> Addresses { get; private set; } = [];

    private Tenant()
    {
        Name = PrimaryEmail = SecondaryEmail = string.Empty;
    }

    public Tenant(string name, string primaryEmail, string secondaryEmail, IEnumerable<Address> addresses)
    {
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        Addresses = addresses.ToList();
    }

    public void Update(string name, string primaryEmail, string secondaryEmail, IEnumerable<Address> addresses)
    {
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        Addresses = addresses.ToList();
    }
}
