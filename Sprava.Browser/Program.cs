using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Inanna.Helpers;

namespace Sprava.Browser;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        DiHelper.ServiceProvider = new ServiceProvider();

        return BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();
}