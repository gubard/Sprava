using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;
using Sprava.Services;

namespace Sprava.Ui;

public partial class PaneViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISpravaViewModelFactory _spravaViewModelFactory;
    private readonly IAppResourceService _appResourceService;

    public PaneViewModel(
        IDialogService dialogService,
        ISpravaViewModelFactory spravaViewModelFactory,
        IAppResourceService appResourceService
    )
    {
        _dialogService = dialogService;
        _spravaViewModelFactory = spravaViewModelFactory;
        _appResourceService = appResourceService;
    }

    [RelayCommand]
    private async Task ShowSettingsViewAsync(CancellationToken ct)
    {
        var setting = _spravaViewModelFactory.Create();

        await WrapCommandAsync(
            () =>
                _dialogService.ShowMessageBoxAsync(
                    new(
                        _appResourceService.GetResource<string>("Lang.Settings"),
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
        await DiHelper
            .ServiceProvider.GetService<ISettingsService<SpravaSettings>>()
            .SaveSettingsAsync(
                new()
                {
                    CromwellSettings = new()
                    {
                        GeneralKey = setting.GeneralKey,
                        Theme = setting.Theme,
                    },
                },
                ct
            );

        Dispatcher.UIThread.Post(() => _dialogService.CloseMessageBox());
    }
}
