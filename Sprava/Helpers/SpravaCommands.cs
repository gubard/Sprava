using System.Windows.Input;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;

namespace Sprava.Helpers;

public static class SpravaCommands
{
    static SpravaCommands()
    {
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();

        var uiAuthenticationService =
            DiHelper.ServiceProvider.GetService<IAuthenticationUiService>();

        async ValueTask LogoutAsync(CancellationToken ct)
        {
            await uiAuthenticationService.LogoutAsync(ct);
            await UiHelper.NavigateToAsync<SignInViewModel>(ct);
        }

        async ValueTask<IValidationErrors> SwitchServiceModeAsync(
            IServiceState state,
            CancellationToken ct
        )
        {
            if (state.Mode == ServiceMode.Online)
            {
                Dispatcher.UIThread.Post(() => state.Mode = ServiceMode.Offline);

                return new DefaultValidationErrors();
            }

            var errors = await state.HealthCheckAsync(ct);

            if (errors.ValidationErrors.Count != 0)
            {
                return errors;
            }

            await navigator.RefreshCurrentViewAsync(ct);

            return errors;
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

        SwitchServiceModeCommand = UiHelper.CreateCommand<IServiceState, IValidationErrors>(
            (state, ct) => SwitchServiceModeAsync(state, ct).ConfigureAwait(false)
        );
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
    public static readonly ICommand LogoutCommand;
    public static readonly ICommand SwitchServiceModeCommand;
}
