using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Markup.Xaml;

namespace CrossPlatformApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                desktop.MainWindow = new Views.LoginWindow();
            }
            catch (Exception ex)
            {
                desktop.MainWindow = CreateFallbackWindow("CrossPlatformApp", BuildErrorText(ex));
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            LogException(exception);
        }
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        LogException(e.Exception);
    }

    private static void LogException(Exception exception)
    {
        var text = BuildErrorText(exception);
        Debug.WriteLine(text);
        Console.Error.WriteLine(text);
    }

    private static string BuildErrorText(Exception exception)
    {
        var baseException = exception.GetBaseException();
        var message = string.IsNullOrWhiteSpace(baseException.Message) ? exception.ToString() : baseException.Message;
        return "Terjadi kesalahan: " + message;
    }

    private static Window CreateFallbackWindow(string title, string message)
    {
        return new Window
        {
            Title = title,
            Width = 720,
            Height = 360,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new Border
            {
                Padding = new Thickness(20),
                Child = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }
}
