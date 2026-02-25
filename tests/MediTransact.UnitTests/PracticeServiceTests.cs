using MediTransact.Application.DTOs;
using MediTransact.Application.Interfaces;
using MediTransact.Application.Services;
using MediTransact.Domain.Entities;
using MediTransact.Domain.Interfaces;
using Moq;

namespace MediTransact.UnitTests;

public class PracticeServiceTests
{
    [Fact]
    public async Task RegisterPatient_ShouldPersistAndReturnId()
    {
        var carrierId = Guid.NewGuid();
        var repo = new Mock<IPracticeRepository>();
        repo.Setup(r => r.GetInsuranceCarrierAsync(carrierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InsuranceCarrier("Aetna", "P123", "a@a.com", "555", "auth@a.com", [new Address(AddressType.Physical, "addr", "", "", "city", "st", "00000", "555", PhoneType.Primary)], []));

        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);

        var result = await sut.RegisterPatientAsync(
            new RegisterPatientRequest(
                "Ana", "Marie", "Cole", new DateOnly(1990, 1, 1), "Female", "DriverLicense", "D123",
                "Single", "555-0101", "ana@example.com", "1 Main", "", "Austin", "TX", "78701",
                "Acme Corp", "555-0202", "hr@acme.com", null, true, new DateOnly(2020, 1, 1), "English", "Hispanic",
                false, null, "",
                [new PatientInsurancePlanInput(carrierId, "GOLD", "G-20", "M-1", "John Rep", "555-3333", "rep@aetna.com", new DateOnly(2024,1,1), null)]),
            CancellationToken.None);

        repo.Verify(r => r.AddPatientAsync(It.Is<Patient>(p => p.FirstName == "Ana" && p.IdentificationNumber == "D123"), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.TryPickT0(out var id, out _));
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task RecordPayment_ShouldUpdateInvoiceAndSave()
    {
        var invoice = new Invoice(Guid.NewGuid(), 100m);
        var repo = new Mock<IPracticeRepository>();
        repo.Setup(r => r.GetInvoiceAsync(invoice.Id, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);

        var result = await sut.RecordPaymentAsync(new RecordPaymentRequest(invoice.Id, 100m), CancellationToken.None);

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.True(result.IsT0);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePatient_ShouldRemoveEntityAndSave()
    {
        var patient = new Patient(
            "Ana", "Marie", "Cole", new DateOnly(1990, 1, 1), "Female", "DriverLicense", "D123",
            "Single", "ana@example.com",
            "Acme Corp", "555-0202", "hr@acme.com", null, true, new DateOnly(2020, 1, 1), "English", "Hispanic",
            false, null, "", [new Address(AddressType.Physical, "1 Main", "", "", "Austin", "TX", "78701", "555-0101", PhoneType.Primary)], []);

        var repo = new Mock<IPracticeRepository>();
        repo.Setup(r => r.GetPatientAsync(patient.Id, It.IsAny<CancellationToken>())).ReturnsAsync(patient);
        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);

        var result = await sut.DeletePatientAsync(patient.Id, CancellationToken.None);

        Assert.True(result.IsT0);
        repo.Verify(r => r.RemovePatient(patient), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTenant_ShouldPersistAndReturnId()
    {
        var repo = new Mock<IPracticeRepository>();
        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);

        var result = await sut.CreateTenantAsync(
            new RegisterTenantRequest("North Practice", "1 Main", "", "Austin", "TX", "78701", "PO Box 100", "", "Austin", "TX", "78702", "555-1111", "", "ops@practice.com", ""),
            CancellationToken.None);

        Assert.True(result.IsT0);
        repo.Verify(r => r.AddTenantAsync(It.Is<Tenant>(x => x.Name == "North Practice"), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePracticeLocation_ShouldPersistAndReturnId()
    {
        var repo = new Mock<IPracticeRepository>();
        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);
        var tenant = new Tenant("Tenant A", "main@tenant.com", "", [new Address(AddressType.Physical, "1 Main", "", "", "Austin", "TX", "78701", "555-1111", PhoneType.Primary), new Address(AddressType.Billing, "PO Box 1", "", "", "Austin", "TX", "78702", "", PhoneType.Secondary)]);
        repo.Setup(r => r.GetTenantAsync(tenant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        var result = await sut.CreatePracticeLocationAsync(
            tenant.Id,
            new RegisterPracticeLocationRequest("Main Clinic", "100 Main", "Suite 1", "Austin", "TX", "78701", "PO Box 10", "", "Austin", "TX", "78702", "555-1111", "", "clinic@example.com", "", true),
            CancellationToken.None);

        Assert.True(result.IsT0);
        repo.Verify(r => r.AddPracticeLocationAsync(It.Is<PracticeLocation>(l => l.TenantId == tenant.Id && l.LocationName == "Main Clinic"), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ScheduleAppointment_ShouldReturnNotFound_WhenLocationMissing()
    {
        var patient = new Patient(
            "Ana", "Marie", "Cole", new DateOnly(1990, 1, 1), "Female", "DriverLicense", "D123",
            "Single", "ana@example.com",
            "Acme Corp", "555-0202", "hr@acme.com", null, true, new DateOnly(2020, 1, 1), "English", "Hispanic",
            false, null, "", [new Address(AddressType.Physical, "1 Main", "", "", "Austin", "TX", "78701", "555-0101", PhoneType.Primary)], []);
        var provider = new Provider("Dr Smith", "Cardiology", "1234567890");
        var repo = new Mock<IPracticeRepository>();
        repo.Setup(r => r.GetPatientAsync(patient.Id, It.IsAny<CancellationToken>())).ReturnsAsync(patient);
        repo.Setup(r => r.GetProviderAsync(provider.Id, It.IsAny<CancellationToken>())).ReturnsAsync(provider);
        repo.Setup(r => r.GetPracticeLocationAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((PracticeLocation?)null);

        var readModel = new Mock<IPracticeReadModel>();
        var sut = new PracticeService(repo.Object, readModel.Object);

        var result = await sut.ScheduleAppointmentAsync(
            new ScheduleAppointmentRequest(patient.Id, provider.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), "Checkup"),
            CancellationToken.None);

        Assert.True(result.IsT1);
        repo.Verify(r => r.AddAppointmentAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

}
