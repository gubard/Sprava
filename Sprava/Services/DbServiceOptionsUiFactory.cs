using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Models;

namespace Sprava.Services;

public sealed class DbServiceOptionsUiFactory : IFactory<DbServiceOptions>
{
    public DbServiceOptionsUiFactory(AppState appState, string serviceName)
    {
        _appState = appState;
        _serviceName = serviceName;
    }

    public DbServiceOptions Create()
    {
        var mode = _appState.GetServiceMode(_serviceName);

        return mode switch
        {
            ServiceMode.Online => new(false),
            ServiceMode.Offline => new(true),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    private readonly AppState _appState;
    private readonly string _serviceName;
}
