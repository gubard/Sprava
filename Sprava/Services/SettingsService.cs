using Cromwell.Models;
using Inanna.Services;
using Melnikov.Models;
using Microsoft.EntityFrameworkCore;
using Nestor.Db.Models;
using Sprava.Models;

namespace Sprava.Services;

public class MelnikovSettingsSettingsService : ISettingsService<MelnikovSettings>
{
    private readonly DbContext _dbContext;

    public MelnikovSettingsSettingsService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<MelnikovSettings> GetSettingsAsync(CancellationToken ct)
    {
        var settings = await MelnikovSettings.FindEntityAsync(
            Guid.Empty,
            _dbContext.Set<EventEntity>(),
            ct
        );

        return settings ?? new();
    }

    public async ValueTask SaveSettingsAsync(MelnikovSettings settings, CancellationToken ct)
    {
        await MelnikovSettings.EditEntitiesAsync(
            _dbContext,
            "App",
            [new(Guid.Empty) { Token = settings.Token, IsEditToken = true }],
            ct
        );

        await _dbContext.SaveChangesAsync(ct);
    }

    public MelnikovSettings GetSettings()
    {
        var settings = MelnikovSettings.FindEntity(Guid.Empty, _dbContext.Set<EventEntity>());

        return settings ?? new();
    }

    public void SaveSettings(MelnikovSettings settings)
    {
        MelnikovSettings.EditEntities(
            _dbContext,
            "App",
            [new(Guid.Empty) { Token = settings.Token, IsEditToken = true }]
        );

        _dbContext.SaveChanges();
    }
}

public class SettingsService : ISettingsService<CromwellSettings>, ISettingsService<SpravaSettings>
{
    private readonly DbContext _dbContext;

    public SettingsService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async ValueTask<CromwellSettings> ISettingsService<CromwellSettings>.GetSettingsAsync(
        CancellationToken ct
    )
    {
        var settings = await CromwellSettings.FindEntityAsync(
            Guid.Empty,
            _dbContext.Set<EventEntity>(),
            ct
        );

        return settings ?? new();
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

    public async ValueTask SaveSettingsAsync(CromwellSettings settings, CancellationToken ct)
    {
        await CromwellSettings.EditEntitiesAsync(
            _dbContext,
            "App",
            [
                new(Guid.Empty)
                {
                    GeneralKey = settings.GeneralKey,
                    IsEditGeneralKey = true,
                    IsEditTheme = true,
                    Theme = settings.Theme,
                },
            ],
            ct
        );

        await _dbContext.SaveChangesAsync(ct);
    }

    CromwellSettings ISettingsService<CromwellSettings>.GetSettings()
    {
        var settings = CromwellSettings.FindEntity(Guid.Empty, _dbContext.Set<EventEntity>());

        return settings ?? new();
    }

    public void SaveSettings(CromwellSettings settings)
    {
        CromwellSettings.EditEntities(
            _dbContext,
            "App",
            [
                new(Guid.Empty)
                {
                    GeneralKey = settings.GeneralKey,
                    IsEditGeneralKey = true,
                    IsEditTheme = true,
                    Theme = settings.Theme,
                },
            ]
        );

        _dbContext.SaveChanges();
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
