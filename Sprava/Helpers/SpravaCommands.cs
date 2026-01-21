using System.Windows.Input;
using Avalonia.Threading;
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

        async ValueTask LogoutAsync(CancellationToken ct)
        {
            await uiAuthenticationService.LogoutAsync(ct);
            await UiHelper.NavigateToAsync<SignInViewModel>(ct);
        }

        ShowPaneCommand = UiHelper.CreateCommand(_ =>
        {
            Dispatcher.UIThread.Post(() => mainViewModel.IsShowPane = true);

            return TaskHelper.ConfiguredCompletedTask;
        });

        HidePaneCommand = UiHelper.CreateCommand(_ =>
        {
            Dispatcher.UIThread.Post(() => mainViewModel.IsShowPane = false);

            return TaskHelper.ConfiguredCompletedTask;
        });

        LogoutCommand = UiHelper.CreateCommand(ct => LogoutAsync(ct).ConfigureAwait(false));
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
    public static readonly ICommand LogoutCommand;
}
