using System.IdentityModel.Tokens.Jwt;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;
using Melnikov.Services;

namespace Sprava.Ui;

public partial class NavigationBarViewModel : ViewModelBase
{
    public NavigationBarViewModel(
        INavigator navigator,
        IAppResourceService appResourceService,
        IUiAuthenticationService authenticationService,
        AppState appState,
        JwtSecurityTokenHandler jwtSecurityTokenHandler
    )
    {
        _navigator = navigator;
        _appResourceService = appResourceService;

        authenticationService.LoggedIn += token =>
        {
            if (!jwtSecurityTokenHandler.CanReadToken(token.Token))
            {
                throw new("Invalid token");
            }

            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(token.Token);

            appState.User = new()
            {
                Id = Guid.Parse(jwtSecurityToken.Claims.GetNameIdentifierClaim().Value),
                Login = jwtSecurityToken.Claims.GetNameClaim().Value,
                Email = jwtSecurityToken.Claims.GetEmailClaim().Value,
            };
        };

        authenticationService.LoggedOut += () =>
        {
            appState.User = null;
        };

        _navigator.ViewChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsCanBack));
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(IsVisible));
        };
    }

    public bool IsCanBack => !_navigator.IsEmpty;
    public bool IsVisible => _navigator.CurrentView is not INonHeader;

    public object Header =>
        _navigator.CurrentView switch
        {
            null => new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Sprava"),
                Classes = { "align-left-center", "h3" },
            },
            IHeader header => header.Header,
            _ => new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Sprava"),
                Classes = { "align-left-center", "h3" },
            },
        };

    private readonly INavigator _navigator;
    private readonly IAppResourceService _appResourceService;

    [RelayCommand]
    private async Task BackAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => BackCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask BackCore(CancellationToken ct)
    {
        await _navigator.NavigateBackOrNullAsync(ct);
        OnPropertyChanged(nameof(IsCanBack));
    }
}
