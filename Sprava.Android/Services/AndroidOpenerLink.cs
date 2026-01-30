using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Android.Content;
using Gaia.Helpers;
using Gaia.Services;
using AndroidUri = Android.Net.Uri;

namespace Sprava.Android.Services;

public sealed class AndroidOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct)
    {
        var intent = new Intent(Intent.ActionView, AndroidUri.Parse(link.AbsoluteUri));
        MainActivity.Activity?.StartActivity(intent);

        return TaskHelper.ConfiguredCompletedTask;
    }
}
