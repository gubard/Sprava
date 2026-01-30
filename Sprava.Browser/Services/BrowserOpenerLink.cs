using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Sprava.Browser.Helpers;

namespace Sprava.Browser.Services;

public sealed class BrowserOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable OpenLinkAsync(Uri link, CancellationToken ct)
    {
        JsWindowInterop.WindowOpen(link.AbsoluteUri);

        return TaskHelper.ConfiguredCompletedTask;
    }
}
