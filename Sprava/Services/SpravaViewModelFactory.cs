using Avalonia;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Sprava.Ui;

namespace Sprava.Services;

public interface ISpravaViewModelFactory : IFactory<AppSettingViewModel>;

public class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(
        Application application,
        IObjectStorage objectStorage,
        AppState appState,
        INavigator navigator
    )
    {
        _application = application;
        _objectStorage = objectStorage;
        _appState = appState;
        _navigator = navigator;
    }

    public AppSettingViewModel Create()
    {
        return new(_application, _objectStorage, _appState, _navigator);
    }

    private readonly Application _application;
    private readonly IObjectStorage _objectStorage;
    private readonly AppState _appState;
    private readonly INavigator _navigator;
}
