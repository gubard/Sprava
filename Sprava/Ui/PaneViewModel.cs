using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Diocles.Models;
using Diocles.Services;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;
using Sprava.Services;

namespace Sprava.Ui;

public partial class PaneViewModel : ViewModelBase
{
    public PaneViewModel(
        IDialogService dialogService,
        ISpravaViewModelFactory spravaViewModelFactory,
        IAppResourceService appResourceService,
        IObjectStorage objectStorage,
        IToDoMemoryCache toDoMemoryCache
    )
    {
        _dialogService = dialogService;
        _spravaViewModelFactory = spravaViewModelFactory;
        _appResourceService = appResourceService;
        _objectStorage = objectStorage;
        ToDos = toDoMemoryCache.Bookmarks;
    }

    public IAvaloniaReadOnlyList<ToDoNotify> ToDos { get; }

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
                        Dispatcher.UIThread.Invoke(() =>
                            _appResourceService
                                .GetResource<string>("Lang.Settings")
                                .ToDialogHeader()
                        ),
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
        await _objectStorage.SaveAsync(
            $"{typeof(SpravaSettings).FullName}",
            new SpravaSettings
            {
                CromwellSettings = new() { GeneralKey = setting.GeneralKey, Theme = setting.Theme },
            },
            ct
        );

        _dialogService.DispatchCloseMessageBox();
    }
}
