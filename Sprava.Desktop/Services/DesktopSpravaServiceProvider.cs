using Gaia.Helpers;
using Jab;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Desktop.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
public partial class DesktopSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig()
    {
        var appSettingsFile = AppDomain
            .CurrentDomain.BaseDirectory.ToDir()
            .ToFile("appsettings.json");

        using var stream = File.OpenRead(appSettingsFile.FullName);

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
