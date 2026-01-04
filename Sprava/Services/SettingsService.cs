using System.Runtime.CompilerServices;
using System.Text.Json;
using Cromwell.Models;
using Gaia.Helpers;
using Inanna.Services;
using Melnikov.Models;
using Sprava.Models;

namespace Sprava.Services;

public class MelnikovSettingsSettingsService : ISettingsService<MelnikovSettings>
{
    private readonly DirectoryInfo _dir;
    private readonly JsonSerializerOptions _jsonOptions;

    public MelnikovSettingsSettingsService(DirectoryInfo dir, JsonSerializerOptions jsonOptions)
    {
        _dir = dir;
        _jsonOptions = jsonOptions;

        if (!_dir.Exists)
        {
            _dir.Create();
        }
    }

    public ConfiguredValueTaskAwaitable<MelnikovSettings> GetSettingsAsync(CancellationToken ct)
    {
        return GetSettingsCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<MelnikovSettings> GetSettingsCore(CancellationToken ct)
    {
        var result = await _dir.ToFile("Melnikov.json")
            .DeserializeJsonAsync<MelnikovSettings>(_jsonOptions, ct);

        return result ?? new();
    }

    public ConfiguredValueTaskAwaitable SaveSettingsAsync(
        MelnikovSettings settings,
        CancellationToken ct
    )
    {
        return _dir.ToFile("Melnikov.json").SerializeJsonAsync(settings, _jsonOptions, ct);
    }

    public MelnikovSettings GetSettings()
    {
        var result = _dir.ToFile("Melnikov.json").DeserializeJson<MelnikovSettings>(_jsonOptions);

        return result ?? new();
    }

    public void SaveSettings(MelnikovSettings settings)
    {
        _dir.ToFile("Melnikov.json").SerializeJson(settings, _jsonOptions);
    }
}

public class SettingsService : ISettingsService<CromwellSettings>, ISettingsService<SpravaSettings>
{
    private readonly DirectoryInfo _dir;
    private readonly JsonSerializerOptions _jsonOptions;

    public SettingsService(DirectoryInfo dir, JsonSerializerOptions jsonOptions)
    {
        _dir = dir;
        _jsonOptions = jsonOptions;
    }

    ConfiguredValueTaskAwaitable<CromwellSettings> ISettingsService<CromwellSettings>.GetSettingsAsync(
        CancellationToken ct
    )
    {
        return GetCromwellSettingsCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<CromwellSettings> GetCromwellSettingsCore(CancellationToken ct)
    {
        var result = await _dir.ToFile("Cromwell.json")
            .DeserializeJsonAsync<CromwellSettings>(_jsonOptions, ct);

        return result ?? new();
    }

    public ConfiguredValueTaskAwaitable SaveSettingsAsync(
        SpravaSettings settings,
        CancellationToken ct
    )
    {
        return SaveSettingsAsync(settings.CromwellSettings, ct);
    }

    SpravaSettings ISettingsService<SpravaSettings>.GetSettings()
    {
        return new()
        {
            CromwellSettings = ((ISettingsService<CromwellSettings>)this).GetSettings(),
        };
    }

    public void SaveSettings(SpravaSettings settings)
    {
        SaveSettings(settings.CromwellSettings);
    }

    public ConfiguredValueTaskAwaitable SaveSettingsAsync(
        CromwellSettings settings,
        CancellationToken ct
    )
    {
        return _dir.ToFile("Cromwell.json").SerializeJsonAsync(settings, _jsonOptions, ct);
    }

    CromwellSettings ISettingsService<CromwellSettings>.GetSettings()
    {
        var result = _dir.ToFile("Cromwell.json").DeserializeJson<CromwellSettings>(_jsonOptions);

        return result ?? new();
    }

    public void SaveSettings(CromwellSettings settings)
    {
        _dir.ToFile("Cromwell.json").SerializeJson(settings, _jsonOptions);
    }

    ConfiguredValueTaskAwaitable<SpravaSettings> ISettingsService<SpravaSettings>.GetSettingsAsync(
        CancellationToken ct
    )
    {
        return GetSpravaSettingsCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<SpravaSettings> GetSpravaSettingsCore(CancellationToken ct)
    {
        return new()
        {
            CromwellSettings = await ((ISettingsService<CromwellSettings>)this).GetSettingsAsync(
                ct
            ),
        };
    }
}
