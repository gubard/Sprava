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
    private readonly SemaphoreSlim _asyncSemaphore = new(1, 1);
    private readonly MemoryStream _stream;
    private readonly UltraLiteDatabase _database;
    private readonly string _fileName;

    public BrowserDatabase(string fileName, UltraLiteDatabase database, MemoryStream stream)
    {
        _fileName = fileName;
        _database = database;
        _stream = stream;
    }

    public void Dispose()
    {
        _database.Dispose();
        _stream.Dispose();
    }

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Action<UltraLiteDatabase> action,
        CancellationToken ct
    )
    {
        return ExecuteCore(action, ct).ConfigureAwait(false);
    }

    private async ValueTask ExecuteCore(Action<UltraLiteDatabase> action, CancellationToken ct)
    {
        await _asyncSemaphore.WaitAsync(ct);

        try
        {
            action(_database);
            var data = _stream.ToArray();
            await JsInterop.SaveDatabase(_fileName, data);
        }
        finally
        {
            _asyncSemaphore.Release();
        }
    }

    public ConfiguredValueTaskAwaitable<T> ExecuteAsync<T>(
        Func<UltraLiteDatabase, T> action,
        CancellationToken ct
    )
    {
        return ExecuteCore(action, ct).ConfigureAwait(false);
    }

    private async ValueTask<T> ExecuteCore<T>(
        Func<UltraLiteDatabase, T> action,
        CancellationToken ct
    )
    {
        T result;
        await _asyncSemaphore.WaitAsync(ct);

        try
        {
            result = action(_database);
            var data = _stream.ToArray();
            await JsInterop.SaveDatabase(_fileName, data);
        }
        finally
        {
            _asyncSemaphore.Release();
        }

        return result;
    }
}
