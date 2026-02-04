using Avalonia;
using Gaia.Services;
using Inanna.Models;
using Sprava.Ui;

namespace Sprava.Services;

public interface ISpravaViewModelFactory : IFactory<AppSettingViewModel>;

public class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(
        Application application,
        IObjectStorage objectStorage,
        AppState appState
    )
    {
        _application = application;
        _objectStorage = objectStorage;
        _appState = appState;
    }

    public AppSettingViewModel Create()
    {
        return new(_application, _objectStorage, _appState);
    }

    private readonly Application _application;
    private readonly IObjectStorage _objectStorage;
    private readonly AppState _appState;
}
