using Avalonia.Controls;
using Avalonia.Interactivity;
using Gaia.Helpers;

namespace Sprava.Ui;

public sealed partial class PaneView : UserControl
{
    public PaneView()
    {
        InitializeComponent();
    }

    public PaneViewModel ViewModel =>
        DataContext as PaneViewModel ?? throw new NullReferenceException();

    private void PaneButtonOnClick(object? sender, RoutedEventArgs e)
    {
        DiHelper.ServiceProvider.GetService<MainViewModel>().IsShowPane = false;
        e.Handled = false;
    }
}
