using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIC.Models;
using SIC.Services;
using SIC.Services.Interfaces;
using SIC.ViewModels;
using SIC.Views;

namespace SIC;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

       Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddDebug(); 
            builder.AddConsole(); 
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddSingleton<IConversionService, ConversionService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<LogsPanelViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ConversionModel>();
        
    }
}