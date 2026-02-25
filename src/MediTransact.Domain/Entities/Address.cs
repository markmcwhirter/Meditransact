namespace MediTransact.Domain.Entities;

public enum AddressType
{
    Physical = 1,
    Billing = 2
}

public enum PhoneType
{
    Primary = 1,
    Secondary = 2
}

public class Address
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public AddressType AddressType { get; private set; }
    public string AddressLine1 { get; private set; }
    public string AddressLine2 { get; private set; }
    public string AddressLine3 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Phone { get; private set; }
    public PhoneType PhoneType { get; private set; }

    public Guid? TenantId { get; private set; }
    public Guid? PracticeLocationId { get; private set; }
    public Guid? InsuranceCarrierId { get; private set; }
    public Guid? PatientId { get; private set; }

    private Address()
    {
        AddressLine1 = AddressLine2 = AddressLine3 = City = State = PostalCode = Phone = string.Empty;
    }

    public Address(
        AddressType addressType,
        string addressLine1,
        string addressLine2,
        string addressLine3,
        string city,
        string state,
        string postalCode,
        string phone,
        PhoneType phoneType)
    {
        AddressType = addressType;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        City = city;
        State = state;
        PostalCode = postalCode;
        Phone = phone;
        PhoneType = phoneType;
    }
}
