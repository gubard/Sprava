using Avalonia.Controls;
using Avalonia.Interactivity;
using Gaia.Helpers;
using Inanna.Services;

namespace Sprava.Ui;

public partial class PaneView : UserControl
{
    private readonly IDialogService _dialogService;

    public PaneView()
    {
        InitializeComponent();
        _dialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
    }

    private void PaneButtonOnClick(object? sender, RoutedEventArgs e)
    {
        _dialogService.CloseMessageBox();
    }
}
