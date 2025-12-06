using Gaia.Helpers;
using Sprava.Services;
using UIKit;

namespace Sprava.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        
        DiHelper.ServiceProvider = new SpravaServiceProvider();
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}