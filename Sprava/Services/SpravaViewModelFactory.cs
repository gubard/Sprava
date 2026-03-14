using Avalonia;
using Cromwell.Services;
using Diocles.Services;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Melnikov.Services;
using Sprava.Models;
using Sprava.Ui;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

public interface ISpravaViewModelFactory
{
    AppSettingViewModel CreateAppSettingViewModel();
    NavigationBarViewModel CreateNavigationBar();
    PaneViewModel CreatePane();
    StatusBarViewModel CreateStatusBar();
    MainViewModel CreateMain();
    DeveloperViewModel CreateDeveloper();
}

public sealed class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AppSettingViewModel CreateAppSettingViewModel()
    {
        return new(
            _serviceProvider.GetService<Application>(),
            _serviceProvider.GetService<IObjectStorage>(),
            _serviceProvider.GetService<AppState>(),
            _serviceProvider.GetService<LangResource>(),
            _serviceProvider.GetService<IEnumerable<DownloadInstallItem>>(),
            _serviceProvider.GetService<InannaCommands>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public NavigationBarViewModel CreateNavigationBar()
    {
        return new(
            _serviceProvider.GetService<INavigator>(),
            _serviceProvider.GetService<IAppResourceService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<SpravaCommands>()
        );
    }

    public PaneViewModel CreatePane()
    {
        return new(
            _serviceProvider.GetService<IDialogService>(),
            _serviceProvider.GetService<ISpravaViewModelFactory>(),
            _serviceProvider.GetService<IAppResourceService>(),
            _serviceProvider.GetService<IObjectStorage>(),
            _serviceProvider.GetService<IToDoUiCache>(),
            _serviceProvider.GetService<ICredentialUiCache>(),
            _serviceProvider.GetService<AppState>(),
            _serviceProvider.GetService<IAuthenticationUiService>(),
            _serviceProvider.GetService<IToDoUiService>(),
            _serviceProvider.GetService<InannaCommands>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<SpravaCommands>(),
            _serviceProvider.GetService<CromwellCommands>(),
            _serviceProvider.GetService<DioclesCommands>()
        );
    }

    public StatusBarViewModel CreateStatusBar()
    {
        return new(_serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public MainViewModel CreateMain()
    {
        return new(
            _serviceProvider.GetService<StackViewModel>(),
            _serviceProvider.GetService<StatusBarViewModel>(),
            _serviceProvider.GetService<PaneViewModel>(),
            _serviceProvider.GetService<NavigationBarViewModel>(),
            _serviceProvider.GetService<INavigator>(),
            _serviceProvider.GetService<IProgressService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider
        );
    }

    public DeveloperViewModel CreateDeveloper()
    {
        return new(
            _serviceProvider.GetService<LogsViewModel>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    private readonly IServiceProvider _serviceProvider;
}
