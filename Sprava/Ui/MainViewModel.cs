using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Melnikov.Ui;

namespace Sprava.Ui;

public sealed partial class MainViewModel : ViewModelBase
{
    public MainViewModel(
        StackViewModel stackViewModel,
        StatusBarViewModel statusBarViewModel,
        PaneViewModel paneViewModel,
        NavigationBarViewModel navigationBarViewModel,
        INavigator navigator,
        IProgressService progressService,
        ISafeExecuteWrapper safeExecuteWrapper,
        Gaia.Services.IServiceProvider serviceProvider
    )
        : base(safeExecuteWrapper)
    {
        Stack = stackViewModel;
        NavigationBar = navigationBarViewModel;
        _navigator = navigator;
        Pane = paneViewModel;
        StatusBar = statusBarViewModel;
        ProgressService = progressService;
        _navigator.ViewChanged += (_, _) => OnPropertyChanged(nameof(IsStatusBarVisible));
        _navigator.NavigateToAsync<SignInViewModel>(serviceProvider, CancellationToken.None);
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
    public PaneViewModel Pane { get; }
    public StatusBarViewModel StatusBar { get; }
    public IProgressService ProgressService { get; }
    public bool IsStatusBarVisible => _navigator.CurrentView is not INonStatusBar;

    [ObservableProperty]
    private bool _isShowPane;

    private readonly INavigator _navigator;
}
