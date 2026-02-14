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
using Melnikov.Services;
using Sprava.Services;

namespace Sprava.Ui;

public sealed partial class PaneViewModel : ViewModelBase
{
    public PaneViewModel(
        IDialogService dialogService,
        ISpravaViewModelFactory spravaViewModelFactory,
        IAppResourceService appResourceService,
        IObjectStorage objectStorage,
        IToDoUiCache toDoUiCache,
        ICredentialUiCache credentialUiCache,
        AppState appState,
        IAuthenticationUiService authenticationUiService,
        IToDoUiService toDoUiService
    )
    {
        _dialogService = dialogService;
        _spravaViewModelFactory = spravaViewModelFactory;
        _appResourceService = appResourceService;
        _objectStorage = objectStorage;
        ToDos = toDoUiCache.Bookmarks;
        Credentials = credentialUiCache.Bookmarks;
        AppState = appState;
        _authenticationUiService = authenticationUiService;
        _toDoUiService = toDoUiService;
    }

    public IAvaloniaReadOnlyList<ToDoNotify> ToDos { get; }
    public IAvaloniaReadOnlyList<CredentialNotify> Credentials { get; }
    public AppState AppState { get; }

    private readonly IDialogService _dialogService;
    private readonly ISpravaViewModelFactory _spravaViewModelFactory;
    private readonly IAppResourceService _appResourceService;
    private readonly IObjectStorage _objectStorage;
    private readonly IAuthenticationUiService _authenticationUiService;
    private readonly IToDoUiService _toDoUiService;

    [RelayCommand]
    private async Task UnbookmarkToDoAsync(ToDoNotify item, CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _toDoUiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        Edits =
                        [
                            new()
                            {
                                Ids = [item.Id],
                                IsBookmark = false,
                                IsEditIsBookmark = true,
                            },
                        ],
                    },
                    ct
                ),
            ct
        );
    }

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

    private async ValueTask SaveSettingsCore(AppSettingViewModel viewModel, CancellationToken ct)
    {
        var cromwellSettings = new CromwellSettings { GeneralKey = viewModel.GeneralKey };
        var inannaSettings = new InannaSettings { Theme = viewModel.Theme, Lang = viewModel.Lang };
        await _dialogService.CloseMessageBoxAsync(ct);
        await _objectStorage.SaveAsync(cromwellSettings, ct);
        await _objectStorage.SaveAsync(inannaSettings, ct);

        await _authenticationUiService.InvokeGlobalAsync(() =>
            _objectStorage.SaveAsync(inannaSettings, ct)
        );
    }
}
