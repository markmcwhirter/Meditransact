namespace MediTransact.Domain.Entities;

public enum AppointmentStatus { Scheduled, CheckedIn, Completed, Cancelled }

public class Appointment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public Guid PatientId { get; private set; }
    public Guid ProviderId { get; private set; }
    public Guid PracticeLocationId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public string Reason { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;

    private Appointment() { Reason = string.Empty; }

    public Appointment(Guid patientId, Guid providerId, Guid practiceLocationId, DateTimeOffset start, DateTimeOffset end, string reason)
    {
        PatientId = patientId;
        ProviderId = providerId;
        PracticeLocationId = practiceLocationId;
        StartTime = start;
        EndTime = end;
        Reason = reason;
    }

    public void Update(Guid patientId, Guid providerId, Guid practiceLocationId, DateTimeOffset start, DateTimeOffset end, string reason, AppointmentStatus status)
    {
        PatientId = patientId;
        ProviderId = providerId;
        PracticeLocationId = practiceLocationId;
        StartTime = start;
        EndTime = end;
        Reason = reason;
        Status = status;
    }

    public void Complete() => Status = AppointmentStatus.Completed;
}
