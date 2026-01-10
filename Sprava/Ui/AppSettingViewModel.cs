using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Sprava.Models;

namespace Sprava.Ui;

public partial class AppSettingViewModel : ViewModelBase, IInitUi
{
    public static readonly string FullAppName =
        $"Sprava {typeof(AppSettingViewModel).Assembly.GetName().Version?.ToString() ?? "0.0.0.0"}";

    [ObservableProperty]
    private string _generalKey;

    [ObservableProperty]
    private ThemeVariantType _theme;

    private readonly IObjectStorage _objectStorage;
    private readonly Application _application;

    public AppSettingViewModel(Application application, IObjectStorage objectStorage)
    {
        _application = application;
        _objectStorage = objectStorage;
        _generalKey = string.Empty;
    }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return WrapCommandAsync(() => InitializedCore(ct).ConfigureAwait(false), ct);
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

    private async ValueTask InitializedCore(CancellationToken ct)
    {
        var settings = await _objectStorage.LoadAsync<SpravaSettings>(
            $"{typeof(SpravaSettings).FullName}",
            ct
        );

        if (settings is null)
        {
            return;
        }

        GeneralKey = settings.CromwellSettings.GeneralKey;
        Theme = settings.CromwellSettings.Theme;
    }
}
