using System.Windows.Input;
using Gaia.Helpers;
using Inanna.Helpers;
using Sprava.Ui;

namespace Sprava.Helpers;

public static class SpravaCommands
{
    static SpravaCommands()
    {
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();

        ShowPaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = true;

            return ValueTask.CompletedTask;
        });

        HidePaneCommand = UiHelper.CreateCommand(_ =>
        {
            mainViewModel.IsShowPane = false;

            return ValueTask.CompletedTask;
        });
    }

    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
}