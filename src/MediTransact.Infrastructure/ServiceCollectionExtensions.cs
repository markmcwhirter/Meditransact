using MediTransact.Application.Interfaces;
using MediTransact.Domain.Interfaces;
using MediTransact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediTransact.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<PracticeDbContext>(opt => opt.UseNpgsql(connectionString));
        services.AddScoped<PracticeRepository>();
        services.AddScoped<IPracticeRepository>(sp => sp.GetRequiredService<PracticeRepository>());
        services.AddScoped<IPracticeReadModel>(sp => sp.GetRequiredService<PracticeRepository>());
        return services;
    }
}
