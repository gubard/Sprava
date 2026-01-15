using Avalonia.Controls;
using Avalonia.Interactivity;
using Gaia.Helpers;

namespace Sprava.Ui;

public partial class PaneView : UserControl
{
    public PaneView()
    {
        InitializeComponent();
    }

    private void PaneButtonOnClick(object? sender, RoutedEventArgs e)
    {
        DiHelper.ServiceProvider.GetService<MainViewModel>().IsShowPane = false;
        e.Handled = false;
    }
}
