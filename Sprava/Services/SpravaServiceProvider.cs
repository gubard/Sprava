using System.Collections.Frozen;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Avalonia;
using Aya.Contract.Helpers;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Cai.Services;
using Cromwell.Services;
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
using Manis.Contract.Models;
using Manis.Contract.Services;
using Melnikov.Services;
using Melnikov.Ui;
using Neotoma.Contract.Helpers;
using Neotoma.Contract.Models;
using Neotoma.Contract.Services;
using Nestor.Db.Helpers;
using Nestor.Db.Services;
using Pheidippides.Services;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;
using Sprava.Models;
using Sprava.Ui;
using Turtle.Contract.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using Weber.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

[ServiceProviderModule]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Import(typeof(IPheidippidesServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(AppState))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(ISpravaViewModelFactory), typeof(SpravaViewModelFactory))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Singleton(typeof(StatusBarViewModel))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(JwtSecurityTokenHandler))]
[Transient(typeof(IFactory<Memory<HttpHeader>>), typeof(HeadersFactory))]
[Transient(typeof(AlarmServiceOptions), Factory = nameof(GetAlarmServiceOptions))]
[Transient(typeof(AuthenticationServiceOptions), Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions), Factory = nameof(GetCredentialServiceOptions))]
[Transient(typeof(ToDoServiceOptions), Factory = nameof(GetToDoServiceOptions))]
[Transient(typeof(FileSystemServiceOptions), Factory = nameof(GetFileSystemServiceOptions))]
[Singleton(typeof(IStringFormater), Factory = nameof(GetStringFormater))]
[Transient(typeof(IStorageService), Factory = nameof(GetStorageService))]
[Singleton(typeof(IMigrator), Factory = nameof(GetMigrator))]
[Singleton(typeof(IFileSystemUiService), Factory = nameof(GetFileSystemUiService))]
[Singleton(typeof(ICredentialUiService), Factory = nameof(GetCredentialUiService))]
[Singleton(typeof(IToDoUiService), Factory = nameof(GetToDoUiService))]
[Transient(typeof(HttpClient), Factory = nameof(GetHttpClient))]
[Transient(typeof(IToDoUiCache), Factory = nameof(GetToDoUiCache))]
[Transient(typeof(ICredentialUiCache), Factory = nameof(GetCredentialUiCache))]
[Transient(typeof(IFileSystemUiCache), Factory = nameof(GetFileSystemUiCache))]
[Transient(typeof(DeveloperViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Transient(typeof(IStatusBarService), typeof(StatusBarService))]
[Transient(typeof(IServiceController), Factory = nameof(GetServiceController))]
[Transient(typeof(IFileStorageUiCache), Factory = nameof(GetFileStorageUiCache))]
[Singleton(typeof(IFileStorageUiService), Factory = nameof(GetFileStorageUiService))]
[Transient(typeof(FileStorageServiceOptions), Factory = nameof(GetFileStorageServiceOptions))]
[Singleton(typeof(IAlarmUiCache), Factory = nameof(GetAlarmUiCache))]
[Transient(typeof(IFactory<DbValues>), typeof(DbValuesUiFactory))]
[Singleton(typeof(IAlarmUiService), Factory = nameof(GetAlarmUiService))]
[Transient(typeof(IAuthenticationService), Factory = nameof(GetAuthenticationService))]
[Singleton(typeof(LangResource), Factory = nameof(GetLangResource))]
[Singleton(typeof(IProgressService), typeof(ProgressService))]
[Singleton(typeof(IErrorDialogFactory), typeof(ErrorDialogFactory))]
[Singleton(
    typeof(ILinearBarcodeSerializerFactory),
    Factory = nameof(GetLinearBarcodeSerializerFactory)
)]
public interface ISpravaServiceProvider : IServiceProvider
{
    public static LangResource GetLangResource(Application app)
    {
        return app.Styles.OfType<LangResource>().First();
    }

    public static IAuthenticationService GetAuthenticationService(
        AuthenticationServiceOptions options,
        HttpClient httpClient
    )
    {
        httpClient.BaseAddress = new(options.Url);

        return new AuthenticationHttpService(
            httpClient,
            new()
            {
                TypeInfoResolver = ManisJsonContext.Resolver,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            },
            new TryPolicyService(3, TimeSpan.FromSeconds(1), FuncHelper<Exception>.Empty),
            EmptyHeadersFactory.Instance
        );
    }

    public static IAlarmUiCache GetAlarmUiCache(
        IAlarmMemoryCache memoryCache,
        IAlarmDbCache dbCache
    )
    {
        return new AlarmUiCache(dbCache, memoryCache);
    }

    public static IAlarmUiService GetAlarmUiService(
        AlarmServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IAlarmUiCache uiCache,
        INavigator navigator,
        HttpClient httpClient,
        IAlarmDbService dbService
    )
    {
        httpClient.BaseAddress = new(options.Url);

        var service = new AlarmUiService(
            new AlarmHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = RoosterJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(AlarmUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(AlarmUiService)
        );

        appState.AddService(service);

        return service;
    }

    public static SignInViewModel GetSignInViewModel(
        IAuthenticationUiService authenticationUiService,
        IObjectStorage objectStorage,
        AppState appState,
        IServiceController serviceController
    )
    {
        return new(
            authenticationUiService,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            objectStorage,
            appState,
            serviceController
        );
    }

    public static ILinearBarcodeSerializerFactory GetLinearBarcodeSerializerFactory()
    {
        return new LinearBarcodeSerializerFactory([
            new UpcALinearBarcodeSerializer(),
            new CodabarLinearBarcodeSerializer(),
        ]);
    }

    public static IServiceController GetServiceController(
        IFileSystemUiService fileSystemUiService,
        ICredentialUiService credentialUiService,
        IToDoUiService toDoUiService,
        IFileStorageUiService fileStorageUiService,
        IAlarmUiService alarmUiService
    )
    {
        return new ServiceController([
            fileSystemUiService,
            credentialUiService,
            toDoUiService,
            fileStorageUiService,
            alarmUiService,
        ]);
    }

    public static IFileStorageUiCache GetFileStorageUiCache(
        IFileStorageMemoryCache memoryCache,
        IFileStorageDbCache dbService
    )
    {
        return new FileStorageUiCache(dbService, memoryCache);
    }

    public static IFileSystemUiCache GetFileSystemUiCache(
        IFileSystemMemoryCache memoryCache,
        IFileSystemDbCache dbCache
    )
    {
        return new FileSystemUiCache(dbCache, memoryCache);
    }

    public static ICredentialUiCache GetCredentialUiCache(
        ICredentialMemoryCache memoryCache,
        ICredentialDbCache dbService
    )
    {
        return new CredentialUiCache(dbService, memoryCache);
    }

    public static IToDoUiCache GetToDoUiCache(IToDoMemoryCache memoryCache, IToDoDbCache dbCache)
    {
        return new ToDoUiCache(dbCache, memoryCache);
    }

    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler();

        return new(handler) { Timeout = TimeSpan.FromSeconds(10) };
    }

    public static IFileStorageUiService GetFileStorageUiService(
        FileStorageServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFileStorageUiCache uiCache,
        INavigator navigator,
        HttpClient httpClient,
        IFileStorageDbService dbService
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
            nameof(FileStorageUiService)
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
        IToDoDbService dbService
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
            dbService,
            uiCache,
            navigator,
            nameof(ToDoUiService)
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
        ICredentialDbService dbService,
        HttpClient httpClient
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
            dbService,
            uiCache,
            navigator,
            nameof(CredentialUiService)
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
        IFileSystemDbService dbService,
        HttpClient httpClient
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
            dbService,
            uiCache,
            navigator,
            nameof(FileSystemUiService)
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

        foreach (var (key, value) in RoosterMigration.Migrations)
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

    public static AlarmServiceOptions GetAlarmServiceOptions(ISpravaConfig configuration)
    {
        return configuration.AlarmService;
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
