using Gaia.Helpers;
using UIKit;
using iOSSpravaServiceProvider = Sprava.iOS.Services.iOSSpravaServiceProvider;

namespace Sprava.iOS;

public sealed class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        DiHelper.ServiceProvider = new iOSSpravaServiceProvider();
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
