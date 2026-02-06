using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;

namespace Sprava.Services;

public sealed class DbValuesUiFactory : IFactory<DbValues>
{
    public DbValuesUiFactory(AppState appState)
    {
        _appState = appState;
    }

    public DbValues Create()
    {
        return new(DateTimeOffset.UtcNow.Offset, _appState.User.ThrowIfNull().Id);
    }

    private readonly AppState _appState;
}
