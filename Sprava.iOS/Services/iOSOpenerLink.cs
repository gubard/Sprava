using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Gaia.Helpers;
using Gaia.Services;

namespace Sprava.iOS.Services;

public sealed class iOSOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct)
    {
        return TaskHelper.ConfiguredCompletedTask;
    }
}
