using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Gaia.Helpers;
using Inanna.Services;
using Sprava.Android.Services;
using Sprava.Helpers;

namespace Sprava.Android;

[Activity(
    Label = "Sprava.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation
        | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode
)]
public sealed class MainActivity : AvaloniaMainActivity<App>
{
    public static MainActivity? Activity;

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        Activity = this;
        DiHelper.ServiceProvider = new AndroidSpravaServiceProvider();

        return base.CustomizeAppBuilder(builder).WithInterFont().WithJetBrainsMonoFont();
    }

    public override void OnBackPressed()
    {
        NavigateBackOrNullAsync();
    }

    private ConfiguredValueTaskAwaitable NavigateBackOrNullAsync()
    {
        return NavigateBackOrNullCore().ConfigureAwait(false);
    }

    private async ValueTask NavigateBackOrNullCore()
    {
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        await navigator.NavigateBackAsync(CancellationToken.None);

        if (navigator.CurrentView is null)
        {
            Finish();
        }
    }
}
