using System.Windows.Input;
using Avalonia.Threading;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Services;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

public sealed class SpravaCommands : Commands
{
    public SpravaCommands(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _showPaneCommand = CreateLazyCommand(_ =>
        {
            Dispatcher.UIThread.Post(() =>
                ServiceProvider.GetService<MainViewModel>().IsShowPane = true
            );

            return TaskHelper.ConfiguredCompletedTask;
        });

        _hidePaneCommand = CreateLazyCommand(_ =>
        {
            Dispatcher.UIThread.Post(() =>
                ServiceProvider.GetService<MainViewModel>().IsShowPane = false
            );

            return TaskHelper.ConfiguredCompletedTask;
        });

        _logoutCommand = CreateLazyCommand(async ct =>
        {
            await ServiceProvider.GetService<IAuthenticationUiService>().LogoutAsync(ct);
            await ServiceProvider
                .GetService<INavigator>()
                .NavigateToAsync<SignInViewModel>(serviceProvider, ct);
        });
    }

    public ICommand ShowPaneCommand => _showPaneCommand.Value;
    public ICommand HidePaneCommand => _hidePaneCommand.Value;
    public ICommand LogoutCommand => _logoutCommand.Value;

    private readonly Lazy<ICommand> _showPaneCommand;
    private readonly Lazy<ICommand> _hidePaneCommand;
    private readonly Lazy<ICommand> _logoutCommand;
}
