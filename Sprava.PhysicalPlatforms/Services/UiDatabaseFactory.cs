using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Nestor.Db.LiteDb.Services;

namespace Sprava.PhysicalPlatforms.Services;

public sealed class UiDatabaseFactory : IDatabaseFactory
{
    public UiDatabaseFactory(AppState appState, IStorageService storageService)
    {
        _appState = appState;
        _storageService = storageService;
        _cache = new();
    }

    public ConfiguredValueTaskAwaitable<IDatabase> CreateAsync(CancellationToken ct)
    {
        var dbFile = CreateDbFile();
        InitDbContext(dbFile);

        return TaskHelper.FromResult(_cache[dbFile.FullName]);
    }

    private readonly AppState _appState;
    private readonly IStorageService _storageService;
    private readonly Dictionary<string, IDatabase> _cache;

    private FileInfo CreateDbFile()
    {
        if (_appState.User is null)
        {
            return new($"{_storageService.GetAppDirectory()}/sprava.litedb");
        }

        return new($"{_storageService.GetAppDirectory()}/{_appState.User.Id}.litedb");
    }

    private void InitDbContext(FileInfo file)
    {
        if (_cache.ContainsKey(file.FullName))
        {
            return;
        }

        _cache.Add(file.FullName, new Database(new(file.FullName)));
    }
}
