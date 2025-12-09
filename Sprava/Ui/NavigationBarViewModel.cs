using System.IdentityModel.Tokens.Jwt;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Melnikov.Services;
using Sprava.Models;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Ui;

public partial class NavigationBarViewModel : ViewModelBase
{
    private readonly INavigator _navigator;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppResourceService _appResourceService;

    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private bool _isSingIn;

    public NavigationBarViewModel(
        INavigator navigator,
        IDialogService dialogService,
        IServiceProvider serviceProvider,
        IAppResourceService appResourceService,
        IUiAuthenticationService authenticationService,
        ITryPolicyService tryPolicyService,
        AppState appState,
        JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        _navigator = navigator;
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;
        _appResourceService = appResourceService;

        tryPolicyService.OnSuccess += () =>
        {
            IsOnline = true;
            appState.Mode = AppMode.Online;
        };

        tryPolicyService.OnError += _ =>
        {
            IsOnline = false;
            appState.Mode = AppMode.Offline;
        };

        authenticationService.LoggedIn += token =>
        {
            if (!jwtSecurityTokenHandler.CanReadToken(token.Token))
            {
                throw new("Invalid token");
            }

            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(token.Token);
            IsSingIn = true;

            appState.User = new()
            {
                Id = Guid.Parse(jwtSecurityToken.Claims.GetNameIdentifierClaim().Value),
                Login = jwtSecurityToken.Claims.GetNameClaim().Value,
                Email = jwtSecurityToken.Claims.GetEmailClaim().Value,
            };
        };

        authenticationService.LoggedOut += () =>
        {
            IsSingIn = false;
            appState.User = null;
        };

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
    private async Task ShowSettingsViewAsync(CancellationToken ct)
    {
        var setting = _serviceProvider.GetService<AppSettingViewModel>();

        await WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _appResourceService.GetResource<string>("Lang.Settings"),
            _serviceProvider.GetService<AppSettingViewModel>(),
            new DialogButton(_appResourceService.GetResource<string>("Lang.Save"), SaveSettingsCommand, setting,
                DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task BackAsync(CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            await _navigator.NavigateBackOrNullAsync(ct);
            OnPropertyChanged(nameof(IsCanBack));
        });
    }

    [RelayCommand]
    private async Task SaveSettingsAsync(AppSettingViewModel setting, CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            await DiHelper.ServiceProvider.GetService<ISettingsService<SpravaSettings>>().SaveSettingsAsync(new()
            {
                CromwellSettings = new()
                {
                    GeneralKey = setting.GeneralKey,
                    Id = Guid.Empty,
                    Theme = setting.Theme,
                },
            }, ct);

            _dialogService.CloseMessageBox();
        });
    }
}