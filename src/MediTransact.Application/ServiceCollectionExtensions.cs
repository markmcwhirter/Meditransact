using MediTransact.Application.Interfaces;
using MediTransact.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediTransact.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPracticeService, PracticeService>();
        return services;
    }
}
