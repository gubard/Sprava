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
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        INavigator navigator,
        PaneViewModel pane,
        StatusBarViewModel statusBar
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        _navigator = navigator;
        Pane = pane;
        StatusBar = statusBar;
        UiHelper.NavigateToAsync<SignInViewModel>(CancellationToken.None);
        _navigator.ViewChanged += (_, _) => OnPropertyChanged(nameof(IsStatusBarVisible));
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
    public PaneViewModel Pane { get; }
    public StatusBarViewModel StatusBar { get; }
    public bool IsStatusBarVisible => _navigator.CurrentView is not INonStatusBar;

    [ObservableProperty]
    private bool _isShowPane;

    private readonly INavigator _navigator;
}
