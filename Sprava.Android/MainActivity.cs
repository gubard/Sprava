using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Gaia.Helpers;
using Inanna.Services;
using Sprava.Android.Services;
using Sprava.Helpers;
using Sprava.Ui;

namespace Sprava.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        DiHelper.ServiceProvider = new AndroidSpravaServiceProvider();

        return base.CustomizeAppBuilder(builder).WithInterFont().WithJetBrainsMonoFont();
    }
}

[Activity(
    Label = "Sprava.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation
        | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode
)]
public sealed class MainActivity : AvaloniaMainActivity
{
    public static MainActivity? Activity;

    public MainActivity()
    {
        Activity = this;
    }

    public override void OnBackPressed()
    {
        NavigateBackOrNullAsync();
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);

        if (!hasFocus)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel((Visual?)Activity?.Content);

        if (topLevel?.InputPane is null)
        {
            return;
        }

        var main = DiHelper.ServiceProvider.GetService<MainViewModel>();

        main.MobileBottomRectangleHeight =
            topLevel.InputPane.State == InputPaneState.Closed
                ? 0
                : topLevel.InputPane.OccludedRect.Height;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        EnsureExactAlarmAccessIfNeeded();
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
