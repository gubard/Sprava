using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell;
using Cromwell.Models;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Sprava.Ui;

public partial class AppSettingViewModel : ViewModelBase, IInitUi
{
    public static readonly string FullAppName =
        $"Sprava {typeof(AppSettingViewModel).Assembly.GetName().Version?.ToString() ?? "0.0.0.0"}";

    public AppSettingViewModel(
        Application application,
        IObjectStorage objectStorage,
        AppState appState
    )
    {
        _application = application;
        _objectStorage = objectStorage;
        AppState = appState;
        _generalKey = string.Empty;
    }

    public AppState AppState { get; }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            async () =>
            {
                var settings = await _objectStorage.LoadAsync<CromwellSettings>(ct);

                Dispatcher.UIThread.Post(() =>
                {
                    GeneralKey = settings.GeneralKey;
                    Theme = settings.Theme;
                });
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

    [ObservableProperty]
    private string _generalKey;

    [ObservableProperty]
    private ThemeVariantType _theme;

    private readonly IObjectStorage _objectStorage;
    private readonly Application _application;
}
