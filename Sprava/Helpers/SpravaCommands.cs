using System.Windows.Input;
using Gaia.Helpers;
using Inanna.Helpers;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;

namespace Sprava.Helpers;

public static class SpravaCommands
{
    static SpravaCommands()
    {
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();

        var uiAuthenticationService =
            DiHelper.ServiceProvider.GetService<IUiAuthenticationService>();

        ShowPaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = true;

            return TaskHelper.ConfiguredCompletedTask;
        });

        HidePaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = false;

            return TaskHelper.ConfiguredCompletedTask;
        });

        LogoutCommand = UiHelper.CreateCommand(ct =>
        {
            uiAuthenticationService.Logout();

            return UiHelper.NavigateToAsync<SignInViewModel>(ct);
        });
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
    public static readonly ICommand LogoutCommand;
}
