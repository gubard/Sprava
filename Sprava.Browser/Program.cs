using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Gaia.Helpers;
using Sprava.Browser.Services;
using Sprava.Helpers;

namespace Sprava.Browser;

internal sealed class Program
{
    private static Task Main(string[] args)
    {
        DiHelper.ServiceProvider = new BrowserSpravaServiceProvider();

        return BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().WithInterFont().WithJetBrainsMonoFont();
    }
}
