using System.Collections.Frozen;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Avalonia;
using Aya.Contract.Helpers;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Aya.Db.Services;
using Cai.Services;
using Cai.Ui;
using Cromwell.Services;
using Cromwell.Ui;
using Diocles.Services;
using Diocles.Ui;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Hestia.Contract.Helpers;
using Hestia.Contract.Models;
using Hestia.Contract.Services;
using Hestia.Db.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Jab;
using Manis.Contract.Models;
using Manis.Contract.Services;
using Melnikov.Services;
using Melnikov.Ui;
using Microsoft.Extensions.Logging;
using Neotoma.Contract.Helpers;
using Neotoma.Contract.Models;
using Neotoma.Contract.Services;
using Neotoma.Db.Services;
using Nestor.Db.Helpers;
using Nestor.Db.LiteDb.Services;
using Nestor.Db.Services;
using Pheidippides.Services;
using Pheidippides.Ui;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;
using Rooster.Db.Services;
using Sprava.Models;
using Sprava.Ui;
using Turtle.Contract.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using Turtle.Db.Services;
using Weber.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

[ServiceProviderModule]
//  Imports
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Import(typeof(IPheidippidesServiceProvider))]
[Import(typeof(IInannaServiceProvider))]
//  Services
[Transient(typeof(ISpravaViewModelFactory), typeof(SpravaViewModelFactory))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Transient(typeof(JwtSecurityTokenHandler))]
[Transient(typeof(IFactory<Memory<HttpHeader>>), typeof(HeadersFactory))]
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
[Transient(typeof(IStatusBarService), typeof(StatusBarService))]
[Transient(typeof(IServiceController), Factory = nameof(GetServiceController))]
[Transient(typeof(IFileStorageUiCache), Factory = nameof(GetFileStorageUiCache))]
[Singleton(typeof(IFileStorageUiService), Factory = nameof(GetFileStorageUiService))]
[Singleton(typeof(IAlarmUiCache), Factory = nameof(GetAlarmUiCache))]
[Transient(typeof(IFactory<DbValues>), typeof(DbValuesUiFactory))]
[Singleton(typeof(IAlarmUiService), Factory = nameof(GetAlarmUiService))]
[Transient(typeof(IAuthenticationService), Factory = nameof(GetAuthenticationService))]
[Singleton(typeof(LangResource), Factory = nameof(GetLangResource))]
[Singleton(typeof(IProgressService), typeof(ProgressService))]
[Singleton(typeof(IErrorDialogFactory), typeof(ErrorDialogFactory))]
[Singleton(typeof(ILogger), Factory = nameof(GetLogger))]
[Singleton(typeof(ILogger<>), Factory = nameof(GetLoggerT))]
[Singleton(typeof(IEnumerable<DownloadInstallItem>), Factory = nameof(GetDownloadInstallItems))]
[Singleton(typeof(ICommandFactory), typeof(CommandFactory))]
[Singleton(typeof(ISafeExecuteWrapper), typeof(SafeExecuteWrapper))]
[Singleton(typeof(SpravaCommands))]
[Singleton(
    typeof(ILinearBarcodeSerializerFactory),
    Factory = nameof(GetLinearBarcodeSerializerFactory)
)]
[Transient(typeof(IFileSystemDbCache), Factory = nameof(GetFileSystemDbService))]
[Transient(typeof(ICredentialDbCache), Factory = nameof(GetCredentialDbService))]
[Transient(typeof(IToDoDbCache), Factory = nameof(GetToDoDbService))]
[Transient(typeof(IFileStorageDbCache), Factory = nameof(GetFileStorageDbService))]
[Singleton(typeof(IAlarmDbCache), Factory = nameof(GetAlarmDbService))]
[Transient(typeof(IFileSystemDbService), Factory = nameof(GetFileSystemDbService))]
[Transient(typeof(ICredentialDbService), Factory = nameof(GetCredentialDbService))]
[Transient(typeof(IToDoDbService), Factory = nameof(GetToDoDbService))]
[Transient(typeof(IFileStorageDbService), Factory = nameof(GetFileStorageDbService))]
[Singleton(typeof(IAlarmDbService), Factory = nameof(GetAlarmDbService))]
[Transient(typeof(IObjectStorage), Factory = nameof(GetObjectStorage))]
//  Models
[Singleton(typeof(AppState))]
[Transient(typeof(AlarmServiceOptions), Factory = nameof(GetAlarmServiceOptions))]
[Transient(typeof(AuthenticationServiceOptions), Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions), Factory = nameof(GetCredentialServiceOptions))]
[Transient(typeof(ToDoServiceOptions), Factory = nameof(GetToDoServiceOptions))]
[Transient(typeof(FileSystemServiceOptions), Factory = nameof(GetFileSystemServiceOptions))]
[Transient(typeof(FileStorageServiceOptions), Factory = nameof(GetFileStorageServiceOptions))]
//  ViewModels
[Singleton(typeof(StackViewModel), Factory = nameof(GetStackViewModel))]
[Singleton(typeof(StatusBarViewModel), Factory = nameof(GetStatusBarViewModel))]
[Singleton(typeof(PaneViewModel), Factory = nameof(GetPaneViewModel))]
[Singleton(typeof(NavigationBarViewModel), Factory = nameof(GetNavigationBarViewModel))]
[Singleton(typeof(MainViewModel), Factory = nameof(GetMainViewModel))]
[Singleton(typeof(LogsViewModel), Factory = nameof(GetLogsViewModel))]
[Transient(typeof(DeveloperViewModel), Factory = nameof(GetDeveloperViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(typeof(SignUpViewModel), Factory = nameof(GetSignUpViewModel))]
[Transient(typeof(RootToDosViewModel), Factory = nameof(GetRootToDosViewModel))]
[Transient(typeof(SearchToDoViewModel), Factory = nameof(GetSearchToDoViewModel))]
[Transient(typeof(RootCredentialsViewModel), Factory = nameof(GetRootCredentialsViewModel))]
[Transient(typeof(AlarmsViewModel), Factory = nameof(GetAlarmsViewModel))]
[Transient(typeof(FilesPanelViewModel), Factory = nameof(GetFilesPanelViewModel))]
public interface ISpravaServiceProvider : IServiceProvider
{
    public static IObjectStorage GetObjectStorage(IDatabaseFactory factory, ISerializer serializer)
    {
        return new LiteDbObjectStorage(factory, serializer);
    }

    public static AlarmLiteDbService GetAlarmDbService(
        AppState appState,
        IDatabaseFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(AlarmUiService))
        );
    }
    public static FileStorageLiteDbService GetFileStorageDbService(
        AppState appState,
        IDatabaseFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(FileStorageUiService))
        );
    }

    public static FileSystemLiteDbService GetFileSystemDbService(
        AppState appState,
        IDatabaseFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(FileSystemUiService))
        );
    }
    public static CredentialLiteDbService GetCredentialDbService(
        AppState appState,
        IDatabaseFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(CredentialUiService))
        );
    }
    public static ToDoLiteDbService GetToDoDbService(
        AppState appState,
        ToDoParametersFillerService parametersFillerService,
        IDatabaseFactory factory,
        IToDoValidator validator,
        IFactory<DbValues> dbValuesFactory
    )
    {
        return new(
            factory,
            dbValuesFactory,
            parametersFillerService,
            validator,
            new DbServiceOptionsUiFactory(appState, nameof(ToDoUiService))
        );
    }

    public static FilesPanelViewModel GetFilesPanelViewModel(ICaiViewModelFactory factory)
    {
        return factory.CreateFilesPanel();
    }

    public static AlarmsViewModel GetAlarmsViewModel(IPheidippidesViewModelFactory factory)
    {
        return factory.CreateAlarms();
    }

    public static RootCredentialsViewModel GetRootCredentialsViewModel(
        ICromwellViewModelFactory factory
    )
    {
        return factory.CreateRootCredentials();
    }

    public static SearchToDoViewModel GetSearchToDoViewModel(IDioclesViewModelFactory factory)
    {
        return factory.CreSearchToDo();
    }

    public static RootToDosViewModel GetRootToDosViewModel(IDioclesViewModelFactory factory)
    {
        return factory.CreateRootToDos();
    }

    public static SignUpViewModel GetSignUpViewModel(IMelnikovViewModelFactory factory)
    {
        return factory.CreateSignUp();
    }

    public static SignInViewModel GetSignInViewModel(
        IMelnikovViewModelFactory factory,
        INavigator navigator,
        IServiceProvider serviceProvider
    )
    {
        return factory.CreateSignIn(ct =>
            navigator.NavigateToAsync<RootToDosViewModel>(serviceProvider, ct)
        );
    }
    public static DeveloperViewModel GetDeveloperViewModel(ISpravaViewModelFactory factory)
    {
        return factory.CreateDeveloper();
    }

    public static LogsViewModel GetLogsViewModel(IInannaViewModelFactory factory)
    {
        return factory.CreateLogs();
    }

    public static StackViewModel GetStackViewModel(IInannaViewModelFactory factory)
    {
        return factory.CreateStack();
    }

    public static MainViewModel GetMainViewModel(ISpravaViewModelFactory factory)
    {
        return factory.CreateMain();
    }

    public static StatusBarViewModel GetStatusBarViewModel(ISpravaViewModelFactory factory)
    {
        return factory.CreateStatusBar();
    }

    public static PaneViewModel GetPaneViewModel(ISpravaViewModelFactory factory)
    {
        return factory.CreatePane();
    }

    public static NavigationBarViewModel GetNavigationBarViewModel(ISpravaViewModelFactory factory)
    {
        return factory.CreateNavigationBar();
    }

    public static IEnumerable<DownloadInstallItem> GetDownloadInstallItems(ISpravaConfig config)
    {
        return config.Downloads;
    }

    public static ILogger GetLogger(IServiceProvider serviceProvider)
    {
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddProvider(new ViewLoggerProvider(serviceProvider))
#if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
#else
                .SetMinimumLevel(LogLevel.Information)
#endif
        );

        var logger = loggerFactory.CreateLogger("App");

        return logger;
    }

    public static ILogger<T> GetLoggerT<T>(IServiceProvider serviceProvider)
    {
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddProvider(new ViewLoggerProvider(serviceProvider))
#if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
#else
                .SetMinimumLevel(LogLevel.Information)
#endif
        );

        var logger = loggerFactory.CreateLogger<T>();

        return logger;
    }

    public static LangResource GetLangResource(Application app)
    {
        return app.Styles.OfType<LangResource>().First();
    }

    public static IAuthenticationService GetAuthenticationService(
        AuthenticationServiceOptions options,
        ILogger logger
    )
    {
        return new AuthenticationHttpService(
            new FuncFactory<HttpClient>(() =>
            {
                var client = GetHttpClient();
                client.BaseAddress = new(options.Url);

                return client;
            }),
            new()
            {
                TypeInfoResolver = ManisJsonContext.Resolver,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            },
            new TryPolicyService(
                3,
                TimeSpan.FromSeconds(1),
                FuncHelper<Exception>.Empty,
                logger,
                FuncHelper<ExceptionsValidationError>.Empty
            ),
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
        IAlarmDbService dbService,
        ILogger logger,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        var service = new AlarmUiService(
            new AlarmHttpService(
                new FuncFactory<HttpClient>(() =>
                {
                    var client = GetHttpClient();
                    client.BaseAddress = new(options.Url);

                    return client;
                }),
                new()
                {
                    TypeInfoResolver = RoosterJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    FuncHelper<Exception>.Empty,
                    logger,
                    _ => appState.SetServiceMode(nameof(AlarmUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(AlarmUiService),
            statusBarService,
            factory
        );

        appState.AddService(service);

        return service;
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

    public static IToDoUiCache GetToDoUiCache(
        IToDoMemoryCache memoryCache,
        IToDoDbCache dbCache,
        DioclesCommands dioclesCommands
    )
    {
        return new ToDoUiCache(dbCache, memoryCache, dioclesCommands);
    }

    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
        return new(handler) { Timeout = TimeSpan.FromSeconds(10) };
    }

    public static IFileStorageUiService GetFileStorageUiService(
        FileStorageServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFileStorageUiCache uiCache,
        INavigator navigator,
        IFileStorageDbService dbService,
        ILogger logger,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        var service = new FileStorageUiService(
            new FileStorageHttpService(
                new FuncFactory<HttpClient>(() =>
                {
                    var client = GetHttpClient();
                    client.BaseAddress = new(options.Url);

                    return client;
                }),
                new()
                {
                    TypeInfoResolver = NeotomaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    FuncHelper<Exception>.Empty,
                    logger,
                    _ => appState.SetServiceMode(nameof(FileStorageUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(FileStorageUiService),
            statusBarService,
            factory
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
        IToDoDbService dbService,
        ILogger logger,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        var service = new ToDoUiService(
            new ToDoHttpService(
                new FuncFactory<HttpClient>(() =>
                {
                    var client = GetHttpClient();
                    client.BaseAddress = new(options.Url);

                    return client;
                }),
                new()
                {
                    TypeInfoResolver = HestiaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    FuncHelper<Exception>.Empty,
                    logger,
                    _ => appState.SetServiceMode(nameof(ToDoUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(ToDoUiService),
            statusBarService,
            factory
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
        ILogger logger,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        var service = new CredentialUiService(
            new CredentialHttpService(
                new FuncFactory<HttpClient>(() =>
                {
                    var client = GetHttpClient();
                    client.BaseAddress = new(options.Url);

                    return client;
                }),
                new()
                {
                    TypeInfoResolver = TurtleJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    FuncHelper<Exception>.Empty,
                    logger,
                    _ => appState.SetServiceMode(nameof(CredentialUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(CredentialUiService),
            statusBarService,
            factory
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
        ILogger logger,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        var service = new FileSystemUiService(
            new FileSystemHttpService(
                new FuncFactory<HttpClient>(() =>
                {
                    var client = GetHttpClient();
                    client.BaseAddress = new(options.Url);

                    return client;
                }),
                new()
                {
                    TypeInfoResolver = AyaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    FuncHelper<Exception>.Empty,
                    logger,
                    _ => appState.SetServiceMode(nameof(FileSystemUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            dbService,
            uiCache,
            navigator,
            nameof(FileSystemUiService),
            statusBarService,
            factory
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

    public static IStorageService GetStorageService(ILogger<StorageService> logger)
    {
        return new StorageService("Sprava", logger);
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
