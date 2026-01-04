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

    public async ValueTask<MelnikovSettings> GetSettingsAsync(CancellationToken ct)
    {
        var result = await _dir.ToFile("Melnikov.json")
            .DeserializeJsonAsync<MelnikovSettings>(_jsonOptions, ct);

        return result ?? new();
    }

    public ValueTask SaveSettingsAsync(MelnikovSettings settings, CancellationToken ct)
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

    async ValueTask<CromwellSettings> ISettingsService<CromwellSettings>.GetSettingsAsync(
        CancellationToken ct
    )
    {
        var result = await _dir.ToFile("Cromwell.json")
            .DeserializeJsonAsync<CromwellSettings>(_jsonOptions, ct);

        return result ?? new();
    }

    public ValueTask SaveSettingsAsync(SpravaSettings settings, CancellationToken ct)
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

    public ValueTask SaveSettingsAsync(CromwellSettings settings, CancellationToken ct)
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

    async ValueTask<SpravaSettings> ISettingsService<SpravaSettings>.GetSettingsAsync(
        CancellationToken ct
    )
    {
        return new()
        {
            CromwellSettings = await ((ISettingsService<CromwellSettings>)this).GetSettingsAsync(
                ct
            ),
        };
    }
}
