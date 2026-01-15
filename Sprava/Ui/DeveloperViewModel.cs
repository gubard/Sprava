using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Inanna.Helpers;
using Inanna.Models;

namespace Sprava.Ui;

public partial class DeveloperViewModel : ViewModelBase
{
    [ObservableProperty]
    private MaterialDesignSizeType _designSizeType;

    [RelayCommand]
    private void Refresh(Button button)
    {
        DesignSizeType = TopLevelAssist.GetMaterialDesignSizeType(
            button.GetVisualAncestors().OfType<TopLevel>().First()
        );
    }
}
