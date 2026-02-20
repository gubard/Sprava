using System;
using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Nestor.Db.Services;
using Sprava.PhysicalPlatforms.Services;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.iOS.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Import(typeof(ISpravaPhysicalPlatformsServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(iOSOpenerLink))]
[Singleton(typeof(IDbConnectionFactory), typeof(UiDbConnectionFactory))]
public sealed partial class iOSSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        var stream = typeof(iOSSpravaServiceProvider)
            .Assembly.GetManifestResourceStream("appsettings.json")
            .ThrowIfNull();

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
