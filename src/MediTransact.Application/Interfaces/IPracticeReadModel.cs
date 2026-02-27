using MediTransact.Application.DTOs;

namespace MediTransact.Application.Interfaces;

public interface IPracticeReadModel
{
    Task<DashboardDto> GetDashboardAsync(DateTime dateUtc, CancellationToken ct);
}
