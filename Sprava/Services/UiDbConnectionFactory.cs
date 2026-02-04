using System.Data.Common;
using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Sprava.Services;

public sealed class UiDbConnectionFactory : IDbConnectionFactory
{
    public UiDbConnectionFactory(
        AppState appState,
        IMigrator migrator,
        IStorageService storageService
    )
    {
        _appState = appState;
        _migrator = migrator;
        _storageService = storageService;
        _factories = new();
    }

    public DbConnection Create()
    {
        var dbFile = CreateDbFile();
        InitDbContext(dbFile);

        return _factories[dbFile].Create();
    }

    private readonly AppState _appState;
    private readonly IMigrator _migrator;
    private readonly IStorageService _storageService;
    private readonly Dictionary<FileInfo, IDbConnectionFactory> _factories;

    private FileInfo CreateDbFile()
    {
        if (_appState.User is null)
        {
            return new($"{_storageService.GetAppDirectory()}/sprava.db");
        }

        return new($"{_storageService.GetAppDirectory()}/{_appState.User.Id}.db");
    }

    private void InitDbContext(FileInfo file)
    {
        if (_factories.ContainsKey(file))
        {
            return;
        }

        var factory = new SqliteDbConnectionFactory(file);
        _migrator.Migrate(factory);
        _factories.Add(file, factory);
    }
}
