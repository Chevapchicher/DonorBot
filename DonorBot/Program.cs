using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BaseBot;

var builder = new ConfigurationBuilder();

builder
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true);

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<AppConfig>().Bind(context.Configuration.GetSection(AppConfig.ConfigKey));
        services.RegisterDependencies();
    })
    .Build();

ActivatorUtilities.CreateInstance<BotHost>(host.Services);

host.Run();