using System;
using System.Net.Http;
using Avalonia.Platform;
using Aya.Contract.Services;
using Gaia.Helpers;
using Gaia.Services;
using Hestia.Contract.Services;
using Jab;
using Neotoma.Contract.Services;
using Rooster.Contract.Services;
using Sprava.Services;
using Turtle.Contract.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Browser.Services;

[ServiceProvider]
[Import(typeof(ISpravaServiceProvider))]
[Singleton(typeof(ISpravaConfig), Factory = nameof(GetSpravaConfig))]
[Singleton(typeof(IOpenerLink), typeof(BrowserOpenerLink))]
[Singleton(typeof(IObjectStorage), typeof(MemoryObjectStorage))]
[Transient(typeof(IFileSystemDbCache), typeof(EmptyFileSystemDbCache))]
[Transient(typeof(ICredentialDbCache), typeof(EmptyCredentialDbCache))]
[Transient(typeof(IToDoDbCache), typeof(EmptyToDoDbCache))]
[Transient(typeof(IFileStorageDbCache), typeof(EmptyFileStorageDbCache))]
[Singleton(typeof(IAlarmDbCache), typeof(EmptyAlarmDbCache))]
[Transient(typeof(IFileSystemDbService), typeof(EmptyFileSystemDbService))]
[Transient(typeof(ICredentialDbService), typeof(EmptyCredentialDbService))]
[Transient(typeof(IToDoDbService), typeof(EmptyToDoDbService))]
[Transient(typeof(IFileStorageDbService), typeof(EmptyFileStorageDbService))]
[Singleton(typeof(IAlarmDbService), typeof(EmptyAlarmDbService))]
public sealed partial class BrowserSpravaServiceProvider : IServiceProvider
{
    public static ISpravaConfig GetSpravaConfig(HttpClient httpClient)
    {
        var uri = new Uri("avares://Sprava.Browser/appsettings.json");
        using var stream = AssetLoader.Open(uri);

        return new SpravaConfig(stream);
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}
