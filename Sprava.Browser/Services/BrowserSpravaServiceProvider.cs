using System;
using System.Net.Http;
using Gaia.Helpers;
using Gaia.Services;
using Jab;
using Sprava.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Browser.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(BrowserOpenerLink))]
public sealed partial class BrowserSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig(HttpClient httpClient)
    {
        using var stream = httpClient
            .GetStreamAsync("appsettings.json")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
