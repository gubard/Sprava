using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using _Microsoft.Android.Resource.Designer;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Services;
using Java.Lang;
using Pheidippides.Models;
using Pheidippides.Services;
using Sprava.Android.Models;

namespace Sprava.Android.Services;

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class AlarmNotificationActionReceiver : BroadcastReceiver
{
    public const string ActionSnooze = "Sprava.Android.Services.ALARM_SNOOZE";

    public override async void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent is null)
        {
            return;
        }

        if (intent.Action != ActionSnooze)
        {
            return;
        }

        var alarmUiService = DiHelper.ServiceProvider.GetService<IAlarmUiService>();

        await alarmUiService.PostAsync(
            Guid.NewGuid(),
            new()
            {
                Edits = [new() { Ids = [Guid.Parse(intent.GetStringExtra("id").ThrowIfNull())] }],
            },
            CancellationToken.None
        );
    }
}

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class AlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent is null)
        {
            return;
        }

        var title = intent.GetStringExtra("title") ?? "Alarm";
        var message = intent.GetStringExtra("message") ?? "Time!";
        ShowNotification(context, title, message);
    }

    private static void ShowNotification(Context context, string title, string message)
    {
        const string channelId = "alarms";

        var notificationManager = (NotificationManager?)
            context.GetSystemService(Context.NotificationService);

        if (notificationManager is null)
        {
            return;
        }

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(channelId, "Alarms", NotificationImportance.High);
            notificationManager.CreateNotificationChannel(channel);
        }

        var snoozeIntent = new Intent(context, typeof(AlarmNotificationActionReceiver));
        snoozeIntent.SetAction(AlarmNotificationActionReceiver.ActionSnooze);

        var snoozePendingIntent = PendingIntent.GetBroadcast(
            context,
            requestCode: 20001,
            snoozeIntent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
        );

        var notification = new NotificationCompat.Builder(context, channelId)
            .SetContentTitle(title)
            .ThrowIfNull()
            .SetContentText(message)
            .ThrowIfNull()
            .SetSmallIcon(ResourceConstant.Drawable.icon)
            .ThrowIfNull()
            .SetPriority((int)NotificationPriority.High)
            .ThrowIfNull()
            .SetCategory(NotificationCompat.CategoryAlarm)
            .ThrowIfNull()
            .SetAutoCancel(true)
            .ThrowIfNull()
            .AddAction(icon: 0, title: "Ok", intent: snoozePendingIntent)
            .ThrowIfNull()
            .Build();

        notificationManager.Notify(JavaSystem.CurrentTimeMillis().GetHashCode(), notification);
    }
}

public sealed class AndroidAlarmScheduler : IAlarmScheduler
{
    public AndroidAlarmScheduler(
        IAppResourceService appResourceService,
        IObjectStorage objectStorage
    )
    {
        _appResourceService = appResourceService;
        _objectStorage = objectStorage;
    }

    public ConfiguredValueTaskAwaitable UpdateAlarmsAsync(
        ReadOnlySpan<AlarmNotify> items,
        CancellationToken ct
    )
    {
        return UpdateAlarmsCore(items.ToArray(), ct).ConfigureAwait(false);
    }

    private readonly IAppResourceService _appResourceService;
    private readonly IObjectStorage _objectStorage;

    private async ValueTask UpdateAlarmsCore(
        ReadOnlyMemory<AlarmNotify> items,
        CancellationToken ct
    )
    {
        var settings = await _objectStorage.LoadAsync<AndroidAlarmSchedulerSettings>(ct);
        var currentCode = settings.RequestCodes.Length == 0 ? 1000 : settings.RequestCodes.Max();

        foreach (var requestCode in settings.RequestCodes)
        {
            Cancel(requestCode);
        }

        settings.RequestCodes = new int[items.Length];

        for (var index = 0; index < items.Span.Length; index++)
        {
            var item = items.Span[index];
            var requestCode = ++currentCode;
            var title = _appResourceService.GetResource<string>("Lang.Alarm");
            ScheduleExact(item, requestCode, title, item.Name);
            settings.RequestCodes[index] = requestCode;
        }

        await _objectStorage.SaveAsync(settings, ct);
    }

    private void ScheduleExact(AlarmNotify item, int requestCode, string title, string message)
    {
        var context = Application.Context;
        var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);

        if (alarmManager is null)
        {
            return;
        }

        var intent = new Intent(context, typeof(AlarmReceiver));
        intent.PutExtra("title", title);
        intent.PutExtra("message", message);
        intent.PutExtra("id", item.Id.ToString());
        var flags = PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable;

        var pendingIntent = PendingIntent
            .GetBroadcast(context, requestCode, intent, flags)
            .ThrowIfNull();

        var triggerAtMillis = item.DueDateTime.ToUnixTimeMilliseconds();
        alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
    }

    private void Cancel(int requestCode)
    {
        var context = Application.Context;
        var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);

        if (alarmManager is null)
        {
            return;
        }

        var intent = new Intent(context, typeof(AlarmReceiver));

        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            requestCode,
            intent,
            PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable
        );

        if (pendingIntent is not null)
        {
            alarmManager.Cancel(pendingIntent);
        }
    }
}
