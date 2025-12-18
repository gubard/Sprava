using System;
using Gaia.Helpers;
using Jab;
using Microsoft.Extensions.Configuration;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Browser.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(IConfiguration), Factory = nameof(GetConfiguration))]
public partial class BrowserSpravaServiceProvider : IServiceProvider
{
    public static IConfiguration GetConfiguration()
    {
        var stream = typeof(BrowserSpravaServiceProvider)
            .Assembly.GetManifestResourceStream("appsettings.json")
            .ThrowIfNull();

        return new ConfigurationBuilder().AddJsonStream(stream).Build();
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
