using System;
using System.Collections.Generic;
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
        _cache = new();
    }

    public ConfiguredValueTaskAwaitable<IDatabase> CreateAsync(CancellationToken ct)
    {
        return CreateCore(ct).ConfigureAwait(false);
    }

    private readonly AppState _appState;
    private readonly Dictionary<string, IDatabase> _cache;

    public async ValueTask<IDatabase> CreateCore(CancellationToken ct)
    {
        var fileName = CreateDbFileName();
        await InitDbContextAsync(fileName, ct);

        return _cache[fileName];
    }

    private string CreateDbFileName()
    {
        if (_appState.User is null)
        {
            return "sprava.litedb";
        }

        return $"{_appState.User.Id}.litedb";
    }

    private async ValueTask InitDbContextAsync(string fileName, CancellationToken ct)
    {
        if (_cache.ContainsKey(fileName))
        {
            return;
        }

        var data = await JsInterop.LoadDatabase(fileName);
        var bytes = DecodeBase64(data);
        var stream = new MemoryStream(bytes);
        stream.Position = 0;
        _cache.Add(fileName, new Database(new(stream)));
    }

    private static byte[] DecodeBase64(string? base64)
    {
        return string.IsNullOrWhiteSpace(base64) ? [] : Convert.FromBase64String(base64);
    }
}
