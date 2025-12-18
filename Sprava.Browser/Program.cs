using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Gaia.Helpers;
using Sprava.Browser.Services;

namespace Sprava.Browser;

internal sealed class Program
{
    private static Task Main(string[] args)
    {
        DiHelper.ServiceProvider = new BrowserSpravaServiceProvider();

        return BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>();
    }
}
