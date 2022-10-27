using System;
using System.Net.Http;
using Athan.Avalonia.Messages;
using Athan.Avalonia.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Athan.Avalonia.ViewModels;
using Athan.Avalonia.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Athan.Avalonia;

internal sealed class App : Application
{
    public new static App Current => (App?) Application.Current!;

    public IServiceProvider Services { get; }

    private IClassicDesktopStyleApplicationLifetime? lifetime;

    public App()
    {
        Services = new ServiceCollection()
            // View-models
            .AddTransient<ShellViewModel>()
            .AddTransient<OfflineViewModel>()
            .AddTransient<LocationViewModel>()
            .AddSingleton<DashboardViewModel>()

            // Views
            .AddTransient<ShellView>()

            // Other
            .AddTransient<PrayerService>()
            .AddTransient<LocationService>()
            .AddTransient<SettingsService>()
            .AddSingleton<NavigationService>()
            .AddSingleton<HttpClient>()
            .BuildServiceProvider();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<ShellView>();
            lifetime = desktop;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OpenTrayIcon(object? sender, EventArgs eventArgs)
    {
        WeakReferenceMessenger.Default.Send<OpenTrayIconMessage>();
    }

    private void CloseTrayIcon(object? sender, EventArgs eventArgs)
    {
        lifetime?.TryShutdown();
    }
}