using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Nestor.Db.Services;
using Pheidippides.Services;
using Sprava.PhysicalPlatforms.Services;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Desktop.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Import(typeof(ISpravaPhysicalPlatformsServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(DesktopOpenerLink))]
[Singleton(typeof(IDbConnectionFactory), typeof(UiDbConnectionFactory))]
[Singleton(typeof(IAlarmScheduler), typeof(DefaultAlarmScheduler))]
[Singleton(typeof(ISoundPlayer), typeof(SoundPlayer))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
public sealed partial class DesktopSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        var appSettingsFile = AppContext.BaseDirectory.ToDir().ToFile("appsettings.json");
        using var stream = appSettingsFile.OpenRead();

        return new SpravaConfig(stream);
    }

    public static ISerializer GetSerializer()
    {
        return new JsonSerializer(SettingsJsonContext.Default.Options);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
