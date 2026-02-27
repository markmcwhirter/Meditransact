namespace MediTransact.Domain.Entities;

public enum InvoiceStatus { Draft, Submitted, PartiallyPaid, Paid, Denied }

public class Invoice
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public Guid PatientId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Submitted;
    public decimal PaidAmount { get; private set; }

    private Invoice() { }

    public Invoice(Guid patientId, decimal totalAmount, Guid? appointmentId = null)
    {
        PatientId = patientId;
        TotalAmount = totalAmount;
        AppointmentId = appointmentId;
    }

    public void Update(Guid patientId, decimal totalAmount, Guid? appointmentId, InvoiceStatus status)
    {
        PatientId = patientId;
        TotalAmount = totalAmount;
        AppointmentId = appointmentId;
        Status = status;
    }

    public void ApplyPayment(decimal amount)
    {
        PaidAmount += amount;
        Status = PaidAmount >= TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
    }
}
