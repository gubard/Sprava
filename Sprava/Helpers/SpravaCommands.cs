using System.Windows.Input;
using Diocles.Ui;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;

namespace Sprava.Helpers;

public static class SpravaCommands
{
    static SpravaCommands()
    {
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();
        var appState = DiHelper.ServiceProvider.GetService<AppState>();

        var uiAuthenticationService =
            DiHelper.ServiceProvider.GetService<IUiAuthenticationService>();

        OfflineCommand = UiHelper.CreateCommand(ct =>
        {
            appState.Mode = AppMode.Offline;

            return UiHelper.NavigateToAsync<RootToDosViewModel>(ct);
        });

        ShowPaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = true;

            return ValueTask.CompletedTask;
        });

        HidePaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = false;

            return ValueTask.CompletedTask;
        });

        LogoutCommand = UiHelper.CreateCommand(ct =>
        {
            uiAuthenticationService.Logout();

            return UiHelper.NavigateToAsync<SignInViewModel>(ct);
        });
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
    public static readonly ICommand OfflineCommand;
    public static readonly ICommand LogoutCommand;
}
