using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Inanna.Helpers;
using Inanna.Models;

namespace Sprava.Ui;

public sealed partial class DeveloperViewModel : ViewModelBase
{
    public IEnumerable<string> Classes => _classes;

    private readonly AvaloniaList<string> _classes = new();

    [ObservableProperty]
    private MaterialDesignSizeType _designSizeType;

    [ObservableProperty]
    private Type? _topLevelType;

    [RelayCommand]
    private void Refresh(Button button)
    {
        var topLevel = button.GetVisualAncestors().OfType<TopLevel>().First();
        TopLevelType = topLevel.GetType();
        _classes.UpdateOrder(topLevel.Classes.ToArray());
        DesignSizeType = TopLevelAssist.GetMaterialDesignSizeType(topLevel);
    }
}
