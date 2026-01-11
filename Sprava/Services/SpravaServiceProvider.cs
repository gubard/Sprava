using System.Collections.Frozen;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Aya.Contract.Helpers;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Cai.Models;
using Cai.Services;
using Cromwell.Models;
using Cromwell.Services;
using Diocles.Models;
using Diocles.Services;
using Diocles.Ui;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Hestia.Contract.Helpers;
using Hestia.Contract.Models;
using Hestia.Contract.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Jab;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Nestor.Db.Helpers;
using Nestor.Db.Services;
using Sprava.Helpers;
using Sprava.Ui;
using Turtle.Contract.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;
using JsonSerializer = Gaia.Services.JsonSerializer;

namespace Sprava.Services;

[ServiceProviderModule]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(AppState))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(typeof(ISpravaViewModelFactory), typeof(SpravaViewModelFactory))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(JwtSecurityTokenHandler))]
[Transient(typeof(IFactory<Memory<HttpHeader>>), typeof(HeadersFactory))]
[Transient(typeof(AuthenticationServiceOptions), Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions), Factory = nameof(GetCredentialServiceOptions))]
[Transient(typeof(ToDoServiceOptions), Factory = nameof(GetToDoServiceOptions))]
[Transient(typeof(GaiaValues), Factory = nameof(GetGaiaValues))]
[Transient(typeof(FilesServiceOptions), Factory = nameof(GetFilesServiceOptions))]
[Singleton(typeof(IStringFormater), Factory = nameof(GetStringFormater))]
[Transient(typeof(IStorageService), Factory = nameof(GetStorageService))]
[Singleton(typeof(IMigrator), Factory = nameof(GetMigrator))]
[Transient(typeof(IUiFilesService), Factory = nameof(GetUiFilesService))]
[Transient(typeof(IUiCredentialService), Factory = nameof(GetUiCredentialService))]
[Transient(typeof(IUiToDoService), Factory = nameof(GetUiToDoService))]
[Transient(typeof(HttpClient), Factory = nameof(GetHttpClient))]
[Transient(typeof(IObjectStorage), Factory = nameof(GetObjectStorage))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
[Transient(typeof(IToDoUiCache), Factory = nameof(GetToDoUiCache))]
[Transient(typeof(DbToDoService), Factory = nameof(GetDbToDoService))]
[Transient(typeof(ICredentialUiCache), Factory = nameof(GetCredentialUiCache))]
[Transient(typeof(DbCredentialService), Factory = nameof(GetDbCredentialService))]
[Transient(typeof(IFilesUiCache), Factory = nameof(GetFilesUiCache))]
[Transient(typeof(DbFilesService), Factory = nameof(GetDbFilesService))]
public interface ISpravaServiceProvider : IServiceProvider
{
    public static DbFilesService GetDbFilesService(
        AppState appState,
        IStorageService storageService,
        IMigrator migrator,
        GaiaValues gaiaValues
    )
    {
        return new(
            new FileInfo(
                $"{storageService.GetAppDirectory()}/{appState.User.ThrowIfNull().Id}.db"
            ).InitDbContext(migrator),
            gaiaValues,
            new DbServiceOptionsUiFactory(appState, nameof(UiCredentialService))
        );
    }

    public static IFilesUiCache GetFilesUiCache(
        IFilesMemoryCache memoryCache,
        DbFilesService dbService
    )
    {
        return new FilesUiCache(dbService, memoryCache);
    }

    public static DbCredentialService GetDbCredentialService(
        AppState appState,
        IStorageService storageService,
        IMigrator migrator,
        GaiaValues gaiaValues
    )
    {
        return new(
            new FileInfo(
                $"{storageService.GetAppDirectory()}/{appState.User.ThrowIfNull().Id}.db"
            ).InitDbContext(migrator),
            gaiaValues,
            new DbServiceOptionsUiFactory(appState, nameof(UiCredentialService))
        );
    }

    public static ICredentialUiCache GetCredentialUiCache(
        ICredentialMemoryCache memoryCache,
        DbCredentialService dbService
    )
    {
        return new CredentialUiCache(dbService, memoryCache);
    }

    public static DbToDoService GetDbToDoService(
        AppState appState,
        ToDoParametersFillerService toDoParametersFillerService,
        IStorageService storageService,
        IToDoValidator toDoValidator,
        IMigrator migrator
    )
    {
        var user = appState.User.ThrowIfNull();

        return new(
            new FileInfo($"{storageService.GetAppDirectory()}/{user.Id}.db").InitDbContext(
                migrator
            ),
            new(DateTimeOffset.UtcNow.Offset, user.Id),
            toDoParametersFillerService,
            toDoValidator,
            new DbServiceOptionsUiFactory(appState, nameof(UiToDoService))
        );
    }

    public static IToDoUiCache GetToDoUiCache(IToDoMemoryCache memoryCache, DbToDoService dbService)
    {
        return new ToDoUiCache(dbService, memoryCache);
    }

    public static ISerializer GetSerializer()
    {
        return new JsonSerializer(SettingsJsonContext.Default.Options);
    }

    public static IObjectStorage GetObjectStorage(
        IStorageService storageService,
        ISerializer serializer
    )
    {
        return new FileObjectStorage(storageService.GetAppDirectory(), serializer);
    }

    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif

        return new(handler) { Timeout = TimeSpan.FromSeconds(10) };
    }

    public static IUiToDoService GetUiToDoService(
        ToDoServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IToDoUiCache uiCache,
        INavigator navigator,
        HttpClient httpClient,
        DbToDoService dbToDoService
    )
    {
        var user = appState.User.ThrowIfNull();
        httpClient.BaseAddress = new(options.Url);

        return new UiToDoService(
            new HttpToDoService(
                httpClient,
                new()
                {
                    TypeInfoResolver = HestiaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(UiToDoService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbToDoService,
            appState,
            uiCache,
            navigator,
            nameof(UiToDoService)
        );
    }

    public static IUiCredentialService GetUiCredentialService(
        CredentialServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        ICredentialUiCache uiCache,
        INavigator navigator,
        DbCredentialService dbService,
        HttpClient httpClient
    )
    {
        httpClient.BaseAddress = new(options.Url);

        return new UiCredentialService(
            new HttpCredentialService(
                httpClient,
                new()
                {
                    TypeInfoResolver = TurtleJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(UiCredentialService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            appState,
            uiCache,
            navigator,
            nameof(UiCredentialService)
        );
    }

    public static IUiFilesService GetUiFilesService(
        FilesServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFilesUiCache uiCache,
        INavigator navigator,
        DbFilesService dbService,
        IMigrator migrator,
        HttpClient httpClient
    )
    {
        httpClient.BaseAddress = new(options.Url);
        var user = appState.User.ThrowIfNull();

        return new UiFilesService(
            new HttpFilesService(
                httpClient,
                new()
                {
                    TypeInfoResolver = AysJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(UiFilesService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            appState,
            uiCache,
            navigator,
            nameof(UiFilesService)
        );
    }

    public static IMigrator GetMigrator()
    {
        var migration = new Dictionary<int, string>();

        foreach (var (key, value) in SqliteMigration.Migrations)
        {
            migration.Add(key, value);
        }

        foreach (var (key, value) in AyaMigration.Migrations)
        {
            migration.Add(key, value);
        }

        foreach (var (key, value) in HestiaMigration.Migrations)
        {
            migration.Add(key, value);
        }

        foreach (var (key, value) in TurtleMigration.Migrations)
        {
            migration.Add(key, value);
        }

        return new Migrator(migration.ToFrozenDictionary());
    }

    public static IStorageService GetStorageService()
    {
        return new StorageService("Sprava");
    }

    public static IStringFormater GetStringFormater()
    {
        return StringFormater.Instance;
    }

    public static GaiaValues GetGaiaValues(AppState appState)
    {
        return new(DateTimeOffset.UtcNow.Offset, appState.User.ThrowIfNull().Id);
    }

    public static SignInViewModel GetSignInViewModel(
        IUiAuthenticationService uiAuthenticationService,
        IObjectStorage objectStorage
    )
    {
        return new(
            uiAuthenticationService,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            objectStorage
        );
    }

    public static AuthenticationServiceOptions GetAuthenticationServiceOptions(
        ISpravaConfig configuration
    )
    {
        return configuration.AuthenticationService;
    }

    public static CredentialServiceOptions GetCredentialServiceOptions(ISpravaConfig configuration)
    {
        return configuration.CredentialService;
    }

    public static ToDoServiceOptions GetToDoServiceOptions(ISpravaConfig configuration)
    {
        return configuration.ToDoService;
    }

    public static FilesServiceOptions GetFilesServiceOptions(ISpravaConfig configuration)
    {
        return configuration.FilesService;
    }
}
