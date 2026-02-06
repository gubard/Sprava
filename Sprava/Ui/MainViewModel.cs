using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Diocles.Services;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Melnikov.Services;

namespace Sprava.Ui;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        INavigator navigator,
        PaneViewModel pane,
        IMelnikovViewModelFactory melnikovFactory,
        StatusBarViewModel statusBar
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        _navigator = navigator;
        Pane = pane;
        StatusBar = statusBar;

        navigator.NavigateToAsync(
            melnikovFactory.CreateSignIn(SignInAsync),
            CancellationToken.None
        );

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

    private ConfiguredValueTaskAwaitable SignInAsync(CancellationToken ct)
    {
        return _navigator.NavigateToAsync(
            DiHelper.ServiceProvider.GetService<IDioclesViewModelFactory>().CreateRootToDos(),
            ct
        );
    }
}
