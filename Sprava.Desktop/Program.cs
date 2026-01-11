using Avalonia;
using Gaia.Helpers;
using Sprava.Desktop.Services;
using Sprava.Helpers;

namespace Sprava.Desktop;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.ServiceProvider = new DesktopSpravaServiceProvider();

        return AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .WithJetBrainsMonoFont();
    }
}
