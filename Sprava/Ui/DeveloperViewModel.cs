using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Ui;

namespace Sprava.Ui;

public sealed partial class DeveloperViewModel : ViewModelBase
{
    public DeveloperViewModel(LogsViewModel logs)
    {
        Logs = logs;
    }

    public IEnumerable<string> Classes => _classes;
    public LogsViewModel Logs { get; }

    [ObservableProperty]
    private MaterialDesignSizeType _designSizeType;

    [ObservableProperty]
    private Type? _topLevelType;

    private readonly AvaloniaList<string> _classes = new();

    [RelayCommand]
    private void Refresh(Button button)
    {
        var topLevel = button.GetVisualAncestors().OfType<TopLevel>().First();
        TopLevelType = topLevel.GetType();
        _classes.UpdateOrder(topLevel.Classes.ToArray());
        DesignSizeType = TopLevelAssist.GetMaterialDesignSizeType(topLevel);
    }
}
