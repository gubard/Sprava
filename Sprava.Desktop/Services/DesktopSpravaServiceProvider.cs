using System;
using Gaia.Helpers;
using Jab;
using Microsoft.Extensions.Configuration;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Desktop.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(IConfiguration), Factory = nameof(GetConfiguration))]
public partial class DesktopSpravaServiceProvider : IServiceProvider
{
    public static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
