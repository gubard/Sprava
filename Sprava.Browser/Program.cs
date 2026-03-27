using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Gaia.Helpers;
using Sprava.Browser.Services;
using Sprava.Helpers;

namespace Sprava.Browser;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        await JSHost.ImportAsync("db-storage.js", "./../db-storage.js");
        await JSHost.ImportAsync("window.js", "./../window.js");
        DiHelper.ServiceProvider = new BrowserSpravaServiceProvider();
        await BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().WithInterFont().WithJetBrainsMonoFont();
    }
}
