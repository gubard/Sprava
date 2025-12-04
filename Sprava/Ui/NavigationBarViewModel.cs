using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Melnikov.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Ui;

public partial class NavigationBarViewModel : ViewModelBase
{
    private readonly INavigator _navigator;
    private readonly IAppSettingService _appSettingService;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IApplicationResourceService _appResourceService;

    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private bool _isSingIn;

    public NavigationBarViewModel(
        INavigator navigator,
        IAppSettingService appSettingService,
        IDialogService dialogService,
        IServiceProvider serviceProvider,
        IApplicationResourceService appResourceService,
        IUiAuthenticationService authenticationService,
        ITryPolicyService tryPolicyService)
    {
        _navigator = navigator;
        _appSettingService = appSettingService;
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;
        _appResourceService = appResourceService;
        tryPolicyService.OnSuccess += () => IsOnline = true;
        tryPolicyService.OnError += _ => IsOnline = false;
        authenticationService.LoggedIn += _ => IsSingIn = true;
        authenticationService.LoggedOut += () => IsSingIn = false;

        _navigator.ViewChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsCanBack));
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(IsVisible));
        };
    }

    public bool IsCanBack
    {
        get => !_navigator.IsEmpty;
    }

    public bool IsVisible
    {
        get => _navigator.CurrentView is not INonHeader;
    }

    public object Header
    {
        get => _navigator.CurrentView switch
        {
            null => new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Sprava"),
                Classes =
                {
                    "alignment-left-center",
                    "h3",
                },
            },
            IHeader header => header.Header,
            _ => new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Sprava"),
                Classes =
                {
                    "alignment-left-center",
                    "h3",
                },
            },
        };
    }

    [RelayCommand]
    private async Task ShowSettingsViewAsync(CancellationToken cancellationToken)
    {
        var setting = _serviceProvider.GetService<AppSettingViewModel>();

        await WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _appResourceService.GetResource<string>("Lang.Settings"),
            _serviceProvider.GetService<AppSettingViewModel>(),
            new DialogButton(_appResourceService.GetResource<string>("Lang.Save"), SaveSettingsCommand, setting,
                DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task BackAsync(CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            await _navigator.NavigateBackOrNullAsync(cancellationToken);
            OnPropertyChanged(nameof(IsCanBack));
        });
    }

    [RelayCommand]
    private async Task SaveSettingsAsync(AppSettingViewModel setting, CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            await _appSettingService.SaveAppSettingsAsync(new()
            {
                GeneralKey = setting.GeneralKey,
                Id = Guid.Empty,
                Theme = setting.Theme,
            }, cancellationToken);

            _dialogService.CloseMessageBox();
        });
    }
}