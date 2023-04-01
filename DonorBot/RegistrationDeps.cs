using DonorBot.Services;
using DonorBot.Services.TelegramBot;
using Microsoft.Extensions.DependencyInjection;

namespace DonorBot;

public static class RegistrationDeps
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddSingleton<TelegramBot>();
        services.AddSingleton<LogService>();
        services.AddSingleton<NotificationService>();
        
        services.AddHostedService<AppointmentCheckerService>();
        services.AddHostedService<ReportService>();
    }
}