using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Sprava.Ui;

public sealed partial class AppSettingViewModel : ViewModelBase, IInit
{
    public static readonly string FullAppName =
        $"Sprava {typeof(AppSettingViewModel).Assembly.GetName().Version?.ToString() ?? "0.0.0.0"}";

    public AppSettingViewModel(
        Application application,
        IObjectStorage objectStorage,
        AppState appState,
        LangResource langResource
    )
    {
        _application = application;
        _objectStorage = objectStorage;
        AppState = appState;
        _langResource = langResource;
        _generalKey = string.Empty;
    }

    public AppState AppState { get; }

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            async () =>
            {
                var cromwellSettings = await _objectStorage.LoadAsync<CromwellSettings>(ct);
                var inannaSettings = await _objectStorage.LoadAsync<InannaSettings>(ct);

                Dispatcher.UIThread.Post(() =>
                {
                    GeneralKey = cromwellSettings.GeneralKey;
                    Theme = inannaSettings.Theme;
                    Lang = inannaSettings.Lang;
                });
            },
            ct
        );
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(Theme):
                _application.RequestedThemeVariant = Theme switch
                {
                    ThemeVariantType.System => ThemeVariant.Default,
                    ThemeVariantType.Dark => ThemeVariant.Dark,
                    ThemeVariantType.Light => ThemeVariant.Light,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                break;
            case nameof(Lang):
                _langResource.Lang = Lang;

                break;
        }
    }

    [ObservableProperty]
    private string _generalKey;

    [ObservableProperty]
    private ThemeVariantType _theme;

    [ObservableProperty]
    private Lang _lang;

    private readonly IObjectStorage _objectStorage;
    private readonly Application _application;
    private readonly LangResource _langResource;
}
