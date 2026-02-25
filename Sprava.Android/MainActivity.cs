using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
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

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        EnsureExactAlarmAccessIfNeeded();
    }

    public override void OnBackPressed()
    {
        NavigateBackOrNullAsync();
    }

    private void EnsureExactAlarmAccessIfNeeded()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.S)
        {
            return;
        }

        if (GetSystemService(AlarmService) is not AlarmManager alarmManager)
        {
            return;
        }

        if (alarmManager.CanScheduleExactAlarms())
        {
            return;
        }

        var intent = new Intent(Settings.ActionRequestScheduleExactAlarm);
        intent.SetData(global::Android.Net.Uri.Parse("package:" + PackageName));
        intent.AddFlags(ActivityFlags.NewTask);

        try
        {
            StartActivity(intent);
        }
        catch
        {
            var fallback = new Intent(Settings.ActionApplicationDetailsSettings);
            fallback.SetData(global::Android.Net.Uri.Parse("package:" + PackageName));
            fallback.AddFlags(ActivityFlags.NewTask);
            StartActivity(fallback);
        }
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
