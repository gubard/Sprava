using Sprava.Android.Services;

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
public class MainActivity : AvaloniaMainActivity<App>
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

        if (await navigator.NavigateBackOrNullAsync(CancellationToken.None) is null)
        {
            base.OnBackPressed();
        }
    }
}
