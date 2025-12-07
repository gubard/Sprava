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

    public ValueTask<MelnikovSettings> GetSettingsAsync(CancellationToken ct)
    {
        return new(new MelnikovSettings());
    }

    public async ValueTask SaveSettingsAsync(MelnikovSettings settings, CancellationToken ct)
    {
        await MelnikovSettings.EditMelnikovSettingssAsync(_dbContext, "App", [
            new(Guid.Empty)
            {
                Token = settings.Token,
                IsEditToken = true,
            },
        ], ct);

        await _dbContext.SaveChangesAsync(ct);
    }
}

public class SettingsService : ISettingsService<CromwellSettings>, ISettingsService<SpravaSettings>
{
    private readonly DbContext _dbContext;

    public SettingsService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async ValueTask<CromwellSettings> ISettingsService<CromwellSettings>.GetSettingsAsync(CancellationToken ct)
    {
        var settings = await CromwellSettings.FindCromwellSettingsAsync(Guid.Empty, _dbContext.Set<EventEntity>(), ct);

        return settings ?? new();
    }

    public ValueTask SaveSettingsAsync(SpravaSettings settings, CancellationToken ct)
    {
        return SaveSettingsAsync(settings.CromwellSettings, ct);
    }

    public async ValueTask SaveSettingsAsync(CromwellSettings settings, CancellationToken ct)
    {
        await CromwellSettings.EditCromwellSettingssAsync(_dbContext, "App", [
            new(Guid.Empty)
            {
                GeneralKey = settings.GeneralKey,
                IsEditGeneralKey = true,
                IsEditTheme = true,
                Theme = settings.Theme,
            },
        ], ct);

        await _dbContext.SaveChangesAsync(ct);
    }

    async ValueTask<SpravaSettings> ISettingsService<SpravaSettings>.GetSettingsAsync(CancellationToken ct)
    {
        return new()
        {
            CromwellSettings = await ((ISettingsService<CromwellSettings>)this).GetSettingsAsync(ct),
        };
    }
}