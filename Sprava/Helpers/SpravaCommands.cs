using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Inanna.Helpers;
using Sprava.Ui;

namespace Sprava.Helpers;

public static class SpravaCommands
{
    static SpravaCommands()
    {
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();

        ShowPaneCommand = new AsyncRelayCommand(ct =>
        {
            mainViewModel.IsShowPane = true;

            return Task.CompletedTask;
        });

        HidePaneCommand = new AsyncRelayCommand(ct =>
        {
            mainViewModel.IsShowPane = false;

            return Task.CompletedTask;
        });
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
}