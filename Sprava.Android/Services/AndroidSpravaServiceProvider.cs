using System;
using Gaia.Helpers;
using Jab;
using Microsoft.Extensions.Configuration;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Android.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(IConfiguration), Factory = nameof(GetConfiguration))]
public partial class AndroidSpravaServiceProvider : IServiceProvider
{
    public static IConfiguration GetConfiguration()
    {
        var stream = typeof(AndroidSpravaServiceProvider)
            .Assembly.GetManifestResourceStream("appsettings.json")
            .ThrowIfNull();

        return new ConfigurationBuilder().AddJsonStream(stream).Build();
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
