using System;
using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Nestor.Db.Services;
using Pheidippides.Services;
using Sprava.PhysicalPlatforms.Services;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Android.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Import(typeof(ISpravaPhysicalPlatformsServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(AndroidOpenerLink))]
[Singleton(typeof(IDbConnectionFactory), typeof(UiDbConnectionFactory))]
[Singleton(typeof(IAlarmScheduler), typeof(AndroidAlarmScheduler))]
public sealed partial class AndroidSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        using var stream = MainActivity
            .Activity.ThrowIfNull()
            .Assets.ThrowIfNull()
            .Open("appsettings.json")
            .ThrowIfNull();

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
