using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Diocles.Models;
using Diocles.Services;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Sprava.Services;

namespace Sprava.Ui;

public partial class PaneViewModel : ViewModelBase
{
    public PaneViewModel(
        IDialogService dialogService,
        ISpravaViewModelFactory spravaViewModelFactory,
        IAppResourceService appResourceService,
        IObjectStorage objectStorage,
        IToDoUiCache toDoUiCache,
        ICredentialUiCache credentialUiCache,
        AppState appState
    )
    {
        _dialogService = dialogService;
        _spravaViewModelFactory = spravaViewModelFactory;
        _appResourceService = appResourceService;
        _objectStorage = objectStorage;
        ToDos = toDoUiCache.Bookmarks;
        Credentials = credentialUiCache.Bookmarks;
        AppState = appState;
    }

    public IAvaloniaReadOnlyList<ToDoNotify> ToDos { get; }
    public IAvaloniaReadOnlyList<CredentialNotify> Credentials { get; }
    public AppState AppState { get; }

    private readonly IDialogService _dialogService;
    private readonly ISpravaViewModelFactory _spravaViewModelFactory;
    private readonly IAppResourceService _appResourceService;
    private readonly IObjectStorage _objectStorage;

    [RelayCommand]
    private async Task ShowSettingsViewAsync(CancellationToken ct)
    {
        var setting = _spravaViewModelFactory.Create();

        await WrapCommandAsync(
            () =>
                _dialogService.ShowMessageBoxAsync(
                    new(
                        _appResourceService
                            .GetResource<string>("Lang.Settings")
                            .DispatchToDialogHeader(),
                        setting,
                        new DialogButton(
                            _appResourceService.GetResource<string>("Lang.Save"),
                            SaveSettingsCommand,
                            setting,
                            DialogButtonType.Primary
                        ),
                        UiHelper.CancelButton
                    ),
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private async Task SaveSettingsAsync(AppSettingViewModel setting, CancellationToken ct)
    {
        await WrapCommandAsync(() => SaveSettingsCore(setting, ct).ConfigureAwait(false), ct);
    }

    private async ValueTask SaveSettingsCore(AppSettingViewModel setting, CancellationToken ct)
    {
        var settings = new CromwellSettings
        {
            GeneralKey = setting.GeneralKey,
            Theme = setting.Theme,
        };

        await _dialogService.CloseMessageBoxAsync(ct);
        await _objectStorage.SaveAsync(settings, ct);
    }
}
