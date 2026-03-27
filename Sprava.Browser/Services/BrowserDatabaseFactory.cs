using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Inanna.Models;
using Nestor.Db.LiteDb.Services;
using Sprava.Browser.Helpers;

namespace Sprava.Browser.Services;

public sealed class BrowserDatabaseFactory : IDatabaseFactory
{
    public BrowserDatabaseFactory(AppState appState)
    {
        _appState = appState;
    }

    public ConfiguredValueTaskAwaitable<IDatabase> CreateAsync(CancellationToken ct)
    {
        return CreateCore(ct).ConfigureAwait(false);
    }

    private readonly AppState _appState;

    private async ValueTask<IDatabase> CreateCore(CancellationToken ct)
    {
        var fileName = GetFileName();
        var data = await JsInterop.LoadDatabase(fileName);
        var bytes = DecodeBase64(data);
        var stream = new MemoryStream(bytes);
        stream.Position = 0;

        return new BrowserDatabase(fileName, new(stream), stream);
    }

    private string GetFileName()
    {
        if (_appState.User is null)
        {
            return "sprava.litedb";
        }

        return $"{_appState.User.Id}.litedb";
    }

    public static byte[] DecodeBase64(string? base64)
    {
        return string.IsNullOrWhiteSpace(base64) ? [] : Convert.FromBase64String(base64);
    }
}
