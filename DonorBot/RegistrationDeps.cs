using DonorBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBot;

public static class RegistrationDeps
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddSingleton<TelegramBot>();
        services.AddSingleton<ReportService>();
        services.AddSingleton<LogService>();
        services.AddSingleton<NotificationService>();
        
        services.AddHostedService<AppointmentCheckerService>();
    }
}