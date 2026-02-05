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
using Inanna.Ui;
using Jab;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Neotoma.Contract.Helpers;
using Neotoma.Contract.Models;
using Neotoma.Contract.Services;
using Nestor.Db.Helpers;
using Nestor.Db.Models;
using Nestor.Db.Services;
using Sprava.Models;
using Sprava.Ui;
using Turtle.Contract.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using Weber.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;
using JsonSerializer = Gaia.Services.JsonSerializer;

namespace Sprava.Services;

[ServiceProviderModule]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(AppState))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(typeof(ISpravaViewModelFactory), typeof(SpravaViewModelFactory))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Singleton(typeof(StatusBarViewModel))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(JwtSecurityTokenHandler))]
[Transient(typeof(IFactory<Memory<HttpHeader>>), typeof(HeadersFactory))]
[Transient(typeof(AuthenticationServiceOptions), Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions), Factory = nameof(GetCredentialServiceOptions))]
[Transient(typeof(ToDoServiceOptions), Factory = nameof(GetToDoServiceOptions))]
[Transient(typeof(GaiaValues), Factory = nameof(GetGaiaValues))]
[Transient(typeof(FileSystemServiceOptions), Factory = nameof(GetFileSystemServiceOptions))]
[Singleton(typeof(IStringFormater), Factory = nameof(GetStringFormater))]
[Transient(typeof(IStorageService), Factory = nameof(GetStorageService))]
[Singleton(typeof(IMigrator), Factory = nameof(GetMigrator))]
[Singleton(typeof(IFileSystemUiService), Factory = nameof(GetFileSystemUiService))]
[Singleton(typeof(ICredentialUiService), Factory = nameof(GetCredentialUiService))]
[Singleton(typeof(IToDoUiService), Factory = nameof(GetToDoUiService))]
[Transient(typeof(HttpClient), Factory = nameof(GetHttpClient))]
[Transient(typeof(IObjectStorage), Factory = nameof(GetObjectStorage))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
[Transient(typeof(IToDoUiCache), Factory = nameof(GetToDoUiCache))]
[Transient(typeof(ToDoDbService), Factory = nameof(GetToDoDbService))]
[Transient(typeof(ICredentialUiCache), Factory = nameof(GetCredentialUiCache))]
[Transient(typeof(CredentialDbService), Factory = nameof(GetCredentialDbService))]
[Transient(typeof(IFileSystemUiCache), Factory = nameof(GetFileSystemUiCache))]
[Transient(typeof(FileSystemDbService), Factory = nameof(GetFileSystemDbService))]
[Singleton(typeof(IDbConnectionFactory), typeof(UiDbConnectionFactory))]
[Transient(typeof(DeveloperViewModel))]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Transient(typeof(IResponseHandler), typeof(ResponseHandler))]
[Transient(typeof(IStatusBarService), typeof(StatusBarService))]
[Transient(typeof(FileStorageDbService), Factory = nameof(GetFileStorageDbService))]
[Transient(typeof(IFileStorageUiCache), Factory = nameof(GetFileStorageUiCache))]
[Singleton(typeof(IFileStorageUiService), Factory = nameof(GetFileStorageUiService))]
[Transient(typeof(FileStorageServiceOptions), Factory = nameof(GetFileStorageServiceOptions))]
public interface ISpravaServiceProvider : IServiceProvider
{
    public static FileStorageDbService GetFileStorageDbService(
        AppState appState,
        IDbConnectionFactory factory,
        GaiaValues gaiaValues
    )
    {
        return new(
            factory,
            gaiaValues,
            new DbServiceOptionsUiFactory(appState, nameof(FileStorageUiService))
        );
    }

    public static FileSystemDbService GetFileSystemDbService(
        AppState appState,
        IDbConnectionFactory factory,
        GaiaValues gaiaValues
    )
    {
        return new(
            factory,
            gaiaValues,
            new DbServiceOptionsUiFactory(appState, nameof(CredentialUiService))
        );
    }

    public static IFileStorageUiCache GetFileStorageUiCache(
        IFileStorageMemoryCache memoryCache,
        FileStorageDbService dbService
    )
    {
        return new FileStorageUiCache(dbService, memoryCache);
    }

    public static IFileSystemUiCache GetFileSystemUiCache(
        IFileSystemMemoryCache memoryCache,
        FileSystemDbService fileSystemDbService
    )
    {
        return new FileSystemSystemUiCache(fileSystemDbService, memoryCache);
    }

    public static CredentialDbService GetCredentialDbService(
        AppState appState,
        IDbConnectionFactory factory,
        GaiaValues gaiaValues
    )
    {
        return new(
            factory,
            gaiaValues,
            new DbServiceOptionsUiFactory(appState, nameof(CredentialUiService))
        );
    }

    public static ICredentialUiCache GetCredentialUiCache(
        ICredentialMemoryCache memoryCache,
        CredentialDbService credentialDbCredentialDbService
    )
    {
        return new CredentialUiCache(credentialDbCredentialDbService, memoryCache);
    }

    public static ToDoDbService GetToDoDbService(
        AppState appState,
        ToDoParametersFillerService toDoParametersFillerService,
        IDbConnectionFactory factory,
        IToDoValidator toDoValidator
    )
    {
        var user = appState.User.ThrowIfNull();

        return new(
            factory,
            new(DateTimeOffset.UtcNow.Offset, user.Id),
            toDoParametersFillerService,
            toDoValidator,
            new DbServiceOptionsUiFactory(appState, nameof(ToDoUiService))
        );
    }

    public static IToDoUiCache GetToDoUiCache(
        IToDoMemoryCache memoryCache,
        ToDoDbService toDoDbService
    )
    {
        return new ToDoUiCache(toDoDbService, memoryCache);
    }

    public static ISerializer GetSerializer()
    {
        return new JsonSerializer(SettingsJsonContext.Default.Options);
    }

    public static IObjectStorage GetObjectStorage(
        IDbConnectionFactory factory,
        ISerializer serializer
    )
    {
        return new DbObjectStorage(factory, serializer);
    }

    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif

        return new(handler) { Timeout = TimeSpan.FromSeconds(10) };
    }

    public static IFileStorageUiService GetFileStorageUiService(
        FileStorageServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFileStorageUiCache uiCache,
        INavigator navigator,
        HttpClient httpClient,
        FileStorageDbService dbService,
        IResponseHandler responseHandler
    )
    {
        httpClient.BaseAddress = new(options.Url);

        var service = new FileStorageUiService(
            new FileStorageHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = NeotomaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(FileStorageUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(FileStorageUiService),
            responseHandler
        );

        appState.AddService(service);

        return service;
    }

    public static IToDoUiService GetToDoUiService(
        ToDoServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IToDoUiCache uiCache,
        INavigator navigator,
        HttpClient httpClient,
        ToDoDbService toDoDbService,
        IResponseHandler responseHandler
    )
    {
        httpClient.BaseAddress = new(options.Url);

        var service = new ToDoUiService(
            new ToDoHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = HestiaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(ToDoUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            toDoDbService,
            uiCache,
            navigator,
            nameof(ToDoUiService),
            responseHandler
        );

        appState.AddService(service);

        return service;
    }

    public static ICredentialUiService GetCredentialUiService(
        CredentialServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        ICredentialUiCache uiCache,
        INavigator navigator,
        CredentialDbService credentialDbCredentialDbService,
        HttpClient httpClient,
        IResponseHandler responseHandler
    )
    {
        httpClient.BaseAddress = new(options.Url);

        var service = new CredentialUiService(
            new CredentialHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = TurtleJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(CredentialUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            credentialDbCredentialDbService,
            uiCache,
            navigator,
            nameof(CredentialUiService),
            responseHandler
        );

        appState.AddService(service);

        return service;
    }

    public static IFileSystemUiService GetFileSystemUiService(
        FileSystemServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFileSystemUiCache uiCache,
        INavigator navigator,
        FileSystemDbService fileSystemDbService,
        HttpClient httpClient,
        IResponseHandler responseHandler
    )
    {
        httpClient.BaseAddress = new(options.Url);

        var service = new FileSystemUiService(
            new FileSystemHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = AyaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(FileSystemUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            fileSystemDbService,
            uiCache,
            navigator,
            nameof(FileSystemUiService),
            responseHandler
        );

        appState.AddService(service);

        return service;
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

        foreach (var (key, value) in NeotomaMigration.Migrations)
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
        IAuthenticationUiService authenticationUiService,
        IObjectStorage objectStorage,
        AppState appState
    )
    {
        return new(
            authenticationUiService,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            objectStorage,
            appState
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

    public static FileSystemServiceOptions GetFileSystemServiceOptions(ISpravaConfig configuration)
    {
        return configuration.FileSystemService;
    }

    public static FileStorageServiceOptions GetFileStorageServiceOptions(
        ISpravaConfig configuration
    )
    {
        return configuration.FileStorageService;
    }
}
