using Avalonia.Controls;

namespace Sprava.Ui;

public partial class AppSettingView : UserControl
{
    public AppSettingView()
    {
        InitializeComponent();
    }

    public AppSettingViewModel ViewModel =>
        DataContext as AppSettingViewModel ?? throw new NullReferenceException();
}
