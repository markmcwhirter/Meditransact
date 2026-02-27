namespace MediTransact.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    public string FirstName { get; private set; }
    public string MiddleName { get; private set; }
    public string LastName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string Sex { get; private set; }
    public string IdentificationType { get; private set; }
    public string IdentificationNumber { get; private set; }
    public string MaritalStatus { get; private set; }
    public string PreferredLanguage { get; private set; }
    public string Ethnicity { get; private set; }

    public string PatientEmail { get; private set; }
    public List<Address> Addresses { get; private set; } = [];

    public string EmployerName { get; private set; }
    public string EmployerPhone { get; private set; }
    public string EmployerEmail { get; private set; }

    public Guid? PrimaryProviderId { get; private set; }
    public bool HipaaConsentAcknowledged { get; private set; }
    public DateOnly? HipaaConsentDate { get; private set; }

    public bool IsDeceased { get; private set; }
    public DateOnly? DeceasedDate { get; private set; }
    public string DeceasedReason { get; private set; }

    public List<PatientInsurancePlan> InsurancePlans { get; private set; } = [];
    public List<PatientCase> Cases { get; private set; } = [];

    private Patient()
    {
        FirstName = MiddleName = LastName = Sex = IdentificationType = IdentificationNumber = string.Empty;
        MaritalStatus = PreferredLanguage = Ethnicity = string.Empty;
        PatientEmail = string.Empty;
        EmployerName = EmployerPhone = EmployerEmail = string.Empty;
        DeceasedReason = string.Empty;
    }

    public Patient(
        string firstName,
        string middleName,
        string lastName,
        DateOnly dateOfBirth,
        string sex,
        string identificationType,
        string identificationNumber,
        string maritalStatus,
        string patientEmail,
        string employerName,
        string employerPhone,
        string employerEmail,
        Guid? primaryProviderId,
        bool hipaaConsentAcknowledged,
        DateOnly? hipaaConsentDate,
        string preferredLanguage,
        string ethnicity,
        bool isDeceased,
        DateOnly? deceasedDate,
        string deceasedReason,
        IEnumerable<Address> addresses,
        IEnumerable<PatientInsurancePlan> insurancePlans)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Sex = sex;
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber;
        MaritalStatus = maritalStatus;
        PatientEmail = patientEmail;
        EmployerName = employerName;
        EmployerPhone = employerPhone;
        EmployerEmail = employerEmail;
        PrimaryProviderId = primaryProviderId;
        HipaaConsentAcknowledged = hipaaConsentAcknowledged;
        HipaaConsentDate = hipaaConsentDate;
        PreferredLanguage = preferredLanguage;
        Ethnicity = ethnicity;
        IsDeceased = isDeceased;
        DeceasedDate = deceasedDate;
        DeceasedReason = deceasedReason;
        Addresses = addresses.ToList();
        InsurancePlans = insurancePlans.ToList();
    }

    public void Update(
        string firstName,
        string middleName,
        string lastName,
        DateOnly dateOfBirth,
        string sex,
        string identificationType,
        string identificationNumber,
        string maritalStatus,
        string patientEmail,
        string employerName,
        string employerPhone,
        string employerEmail,
        Guid? primaryProviderId,
        bool hipaaConsentAcknowledged,
        DateOnly? hipaaConsentDate,
        string preferredLanguage,
        string ethnicity,
        bool isDeceased,
        DateOnly? deceasedDate,
        string deceasedReason,
        IEnumerable<Address> addresses,
        IEnumerable<PatientInsurancePlan> insurancePlans)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Sex = sex;
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber;
        MaritalStatus = maritalStatus;
        PatientEmail = patientEmail;
        EmployerName = employerName;
        EmployerPhone = employerPhone;
        EmployerEmail = employerEmail;
        PrimaryProviderId = primaryProviderId;
        HipaaConsentAcknowledged = hipaaConsentAcknowledged;
        HipaaConsentDate = hipaaConsentDate;
        PreferredLanguage = preferredLanguage;
        Ethnicity = ethnicity;
        IsDeceased = isDeceased;
        DeceasedDate = deceasedDate;
        DeceasedReason = deceasedReason;
        Addresses = addresses.ToList();
        InsurancePlans = insurancePlans.ToList();
    }
}
