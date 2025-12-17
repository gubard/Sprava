using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;

namespace Sprava.Ui;

public partial class AppSettingViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _generalKey;

    [ObservableProperty]
    private ThemeVariantType _theme;

    private readonly ISettingsService<SpravaSettings> _settingsService;
    private readonly Application _application;

    public AppSettingViewModel(
        Application application,
        ISettingsService<SpravaSettings> settingsService
    )
    {
        _application = application;
        _settingsService = settingsService;
        _generalKey = string.Empty;
    }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                var settings = await _settingsService.GetSettingsAsync(ct);
                GeneralKey = settings.CromwellSettings.GeneralKey;
                Theme = settings.CromwellSettings.Theme;
            },
            ct
        );
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Theme))
        {
            _application.RequestedThemeVariant = Theme switch
            {
                ThemeVariantType.Default => ThemeVariant.Default,
                ThemeVariantType.Dark => ThemeVariant.Dark,
                ThemeVariantType.Light => ThemeVariant.Light,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
