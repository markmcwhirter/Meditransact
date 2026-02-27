namespace MediTransact.Domain.Entities;

public class Provider
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public string FullName { get; private set; }
    public string Specialty { get; private set; }
    public string Npi { get; private set; }

    private Provider() { FullName = Specialty = Npi = string.Empty; }

    public Provider(string fullName, string specialty, string npi)
    {
        FullName = fullName;
        Specialty = specialty;
        Npi = npi;
    }

    public void Update(string fullName, string specialty, string npi)
    {
        FullName = fullName;
        Specialty = specialty;
        Npi = npi;
    }
}
