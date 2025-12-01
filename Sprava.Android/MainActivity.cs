using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Gaia.Helpers;
using Sprava.Services;

namespace Sprava.Android;

[Activity(Label = "Sprava.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        DiHelper.ServiceProvider = new SpravaServiceProvider();

        return base.CustomizeAppBuilder(builder).WithInterFont();
    }
}