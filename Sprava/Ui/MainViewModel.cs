using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;
using Inanna.Ui;
using Melnikov.Ui;

namespace Sprava.Ui;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isShowPane;

    public MainViewModel(
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        SignInViewModel signInViewModel,
        PaneViewModel pane
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        Pane = pane;
        Stack.PushView(signInViewModel);
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
    public PaneViewModel Pane { get; }
}
