using Gaia.Services;
using Inanna.Models;
using Nestor.Db.LiteDb.Services;
using UltraLiteDB;

namespace Sprava.PhysicalPlatforms.Services;

public sealed class FileUiUltraLiteDatabaseFactory : IUltraLiteDatabaseFactory
{
    public FileUiUltraLiteDatabaseFactory(AppState appState, IStorageService storageService)
    {
        _appState = appState;
        _storageService = storageService;
        _factories = new();
    }

    public UltraLiteDatabase Create()
    {
        var dbFile = CreateDbFile();
        InitDbContext(dbFile);

        return _factories[dbFile].Create();
    }

    private readonly AppState _appState;
    private readonly IStorageService _storageService;
    private readonly Dictionary<FileInfo, FileUltraLiteDatabaseFactory> _factories;

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
        if (_factories.ContainsKey(file))
        {
            return;
        }

        _factories.Add(file, new(file));
    }
}
