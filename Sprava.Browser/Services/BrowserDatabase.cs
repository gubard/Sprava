using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nestor.Db.LiteDb.Services;
using Sprava.Browser.Helpers;
using UltraLiteDB;

namespace Sprava.Browser.Services;

public sealed class BrowserDatabase : IDatabase
{
    public BrowserDatabase(UltraLiteDatabase database, string fileName, MemoryStream stream)
    {
        _database = database;
        _fileName = fileName;
        _stream = stream;
    }

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Action<UltraLiteDatabase> action,
        CancellationToken ct
    )
    {
        return ExecuteCore(action, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<T> ExecuteAsync<T>(
        Func<UltraLiteDatabase, T> action,
        CancellationToken ct
    )
    {
        return ExecuteCore(action, ct).ConfigureAwait(false);
    }

    private readonly UltraLiteDatabase _database;
    private readonly SemaphoreSlim _asyncSemaphore = new(1, 1);
    private readonly string _fileName;
    private readonly MemoryStream _stream;

    private async ValueTask ExecuteCore(Action<UltraLiteDatabase> action, CancellationToken ct)
    {
        await _asyncSemaphore.WaitAsync(ct);

        try
        {
            action(_database);
            await SaveDatabaseAsync();
        }
        finally
        {
            _asyncSemaphore.Release();
        }
    }

    private async ValueTask<T> ExecuteCore<T>(
        Func<UltraLiteDatabase, T> action,
        CancellationToken ct
    )
    {
        await _asyncSemaphore.WaitAsync(ct);

        try
        {
            var result = action(_database);
            await SaveDatabaseAsync();
            return result;
        }
        finally
        {
            _asyncSemaphore.Release();
        }
    }

    private async ValueTask SaveDatabaseAsync()
    {
        var position = _stream.Position;
        _stream.Position = 0;
        await JsInterop.SaveDatabase(_fileName, _stream.ToArray());
        _stream.Position = position;
    }
}
