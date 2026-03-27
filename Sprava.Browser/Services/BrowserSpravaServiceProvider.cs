using System;
using Avalonia.Platform;
using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Nestor.Db.LiteDb.Services;
using Pheidippides.Services;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Browser.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(BrowserOpenerLink))]
[Singleton(typeof(IAlarmScheduler), typeof(DefaultAlarmScheduler))]
[Singleton(typeof(ISoundPlayer), typeof(SoundPlayer))]
[Singleton(typeof(IDatabaseFactory), typeof(BrowserDatabaseFactory))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
public sealed partial class BrowserSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        var uri = new Uri("avares://Sprava.Browser/appsettings.json");
        using var stream = AssetLoader.Open(uri);

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
