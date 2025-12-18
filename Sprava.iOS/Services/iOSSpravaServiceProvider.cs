using System;
using Gaia.Helpers;
using Jab;
using Microsoft.Extensions.Configuration;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.iOS.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
public partial class iOSSpravaServiceProvider : IServiceProvider
{
    public static IConfiguration GetConfiguration()
    {
        var stream = typeof(iOSSpravaServiceProvider)
            .Assembly.GetManifestResourceStream("appsettings.json")
            .ThrowIfNull();

        return new ConfigurationBuilder().AddJsonStream(stream).Build();
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
