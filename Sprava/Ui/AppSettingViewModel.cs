using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;

namespace Sprava.Ui;

public partial class AppSettingViewModel : ViewModelBase
{
    public static readonly string FullAppName =
        $"Sprava {typeof(AppSettingViewModel).Assembly.GetName().Version?.ToString() ?? "0.0.0.0"}";

    [ObservableProperty]
    private string _generalKey;

    [ObservableProperty]
    private ThemeVariantType _theme;

    private readonly ISettingsService<SpravaSettings> _settingsService;
    private readonly Application _application;
    private readonly IStorageService _storageService;

    public AppSettingViewModel(
        Application application,
        ISettingsService<SpravaSettings> settingsService,
        IStorageService storageService
    )
    {
        _application = application;
        _settingsService = settingsService;
        _storageService = storageService;
        _generalKey = string.Empty;
    }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => InitializedCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask InitializedCore(CancellationToken ct)
    {
        var settings = await _settingsService.GetSettingsAsync(ct);
        GeneralKey = settings.CromwellSettings.GeneralKey;
        Theme = settings.CromwellSettings.Theme;
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
