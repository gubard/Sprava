using System;
using System.Text.Json.Serialization.Metadata;
using Avalonia.Platform;
using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Nestor.Db.Services;
using Pheidippides.Services;
using Sprava.PhysicalPlatforms.Services;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;
using JsonSerializer = Gaia.Services.JsonSerializer;

namespace Sprava.Android.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Import(typeof(ISpravaPhysicalPlatformsServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(AndroidOpenerLink))]
[Singleton(typeof(IDbConnectionFactory), typeof(UiDbConnectionFactory))]
[Singleton(typeof(IAlarmScheduler), typeof(AndroidAlarmScheduler))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
public sealed partial class AndroidSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        var uri = new Uri("avares://Sprava.Android/appsettings.json");
        using var stream = AssetLoader.Open(uri);

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }

    public static ISerializer GetSerializer()
    {
        return new JsonSerializer(
            new()
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(
                    AndroidJsonContext.Default,
                    SettingsJsonContext.Default
                ),
            }
        );
    }
}
