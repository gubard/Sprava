using CommunityToolkit.Mvvm.ComponentModel;

namespace Sprava.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
}