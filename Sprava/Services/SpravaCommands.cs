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

public sealed class SpravaCommands
{
    public SpravaCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _showPaneCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(_ =>
                {
                    Dispatcher.UIThread.Post(() =>
                        _serviceProvider.GetService<MainViewModel>().IsShowPane = true
                    );

                    return TaskHelper.ConfiguredCompletedTask;
                })
        );

        _hidePaneCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(_ =>
                {
                    Dispatcher.UIThread.Post(() =>
                        _serviceProvider.GetService<MainViewModel>().IsShowPane = false
                    );

                    return TaskHelper.ConfiguredCompletedTask;
                })
        );

        _logoutCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(async ct =>
                {
                    await _serviceProvider.GetService<IAuthenticationUiService>().LogoutAsync(ct);
                    await _serviceProvider
                        .GetService<INavigator>()
                        .NavigateToAsync<SignInViewModel>(serviceProvider, ct);
                })
        );
    }

    public ICommand ShowPaneCommand => _showPaneCommand.Value;
    public ICommand HidePaneCommand => _hidePaneCommand.Value;
    public ICommand LogoutCommand => _logoutCommand.Value;

    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<ICommand> _showPaneCommand;
    private readonly Lazy<ICommand> _hidePaneCommand;
    private readonly Lazy<ICommand> _logoutCommand;
}
