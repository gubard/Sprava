using Avalonia;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;
using Sprava.Ui;

namespace Sprava.Services;

public interface ISpravaViewModelFactory : IFactory<AppSettingViewModel>;

public sealed class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(
        Application application,
        IObjectStorage objectStorage,
        AppState appState,
        LangResource langResource,
        IEnumerable<DownloadInstallItem> downloadInstallItems
    )
    {
        _application = application;
        _objectStorage = objectStorage;
        _appState = appState;
        _langResource = langResource;
        _downloadInstallItems = downloadInstallItems;
    }

    public AppSettingViewModel Create()
    {
        return new(
            _application,
            _objectStorage,
            _appState,
            _langResource,
            _downloadInstallItems,
            _inannaCommands
        );
    }

    private readonly Application _application;
    private readonly IObjectStorage _objectStorage;
    private readonly AppState _appState;
    private readonly LangResource _langResource;
    private readonly IEnumerable<DownloadInstallItem> _downloadInstallItems;
    private readonly InannaCommands _inannaCommands;
}
