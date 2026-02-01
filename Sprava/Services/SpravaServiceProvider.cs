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
using Nestor.Db.Models;
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
[Transient(typeof(IFilesUiService), Factory = nameof(GetUiFilesService))]
[Transient(typeof(ICredentialUiService), Factory = nameof(GetUiCredentialService))]
[Transient(typeof(IToDoUiService), Factory = nameof(GetUiToDoService))]
[Transient(typeof(HttpClient), Factory = nameof(GetHttpClient))]
[Transient(typeof(IObjectStorage), Factory = nameof(GetObjectStorage))]
[Transient(typeof(ISerializer), Factory = nameof(GetSerializer))]
[Transient(typeof(IToDoUiCache), Factory = nameof(GetToDoUiCache))]
[Transient(typeof(ToDoDbService), Factory = nameof(GetDbToDoService))]
[Transient(typeof(ICredentialUiCache), Factory = nameof(GetCredentialUiCache))]
[Transient(typeof(CredentialDbService), Factory = nameof(GetDbCredentialService))]
[Transient(typeof(IFileSystemUiCache), Factory = nameof(GetFilesUiCache))]
[Transient(typeof(FileSystemSystemDbService), Factory = nameof(GetDbFilesService))]
[Transient(typeof(IDbConnectionFactory), Factory = nameof(GetDbConnectionFactory))]
[Transient(typeof(DeveloperViewModel))]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Transient(typeof(IResponseHandler), typeof(ResponseHandler))]
public interface ISpravaServiceProvider : IServiceProvider
{
    public static IDbConnectionFactory GetDbConnectionFactory(
        AppState appState,
        IStorageService storageService,
        IMigrator migrator
    )
    {
        if (appState.User is null)
        {
            return new FileInfo($"{storageService.GetAppDirectory()}/sprava.db").InitDbContext(
                migrator
            );
        }

        return new FileInfo(
            $"{storageService.GetAppDirectory()}/{appState.User.Id}.db"
        ).InitDbContext(migrator);
    }

    public static FileSystemSystemDbService GetDbFilesService(
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

    public static IFileSystemUiCache GetFilesUiCache(
        IFileSystemMemoryCache memoryCache,
        FileSystemSystemDbService fileSystemSystemDbService
    )
    {
        return new FileSystemSystemUiCache(fileSystemSystemDbService, memoryCache);
    }

    public static CredentialDbService GetDbCredentialService(
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

    public static ToDoDbService GetDbToDoService(
        AppState appState,
        ToDoParametersFillerService toDoParametersFillerService,
        IDbConnectionFactory factory,
        IToDoValidator toDoValidator,
        IMigrator migrator
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

    public static IToDoUiService GetUiToDoService(
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

        return new ToDoUiService(
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
            appState,
            uiCache,
            navigator,
            nameof(ToDoUiService),
            responseHandler
        );
    }

    public static ICredentialUiService GetUiCredentialService(
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

        return new CredentialUiService(
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
            appState,
            uiCache,
            navigator,
            nameof(CredentialUiService),
            responseHandler
        );
    }

    public static IFilesUiService GetUiFilesService(
        FilesServiceOptions options,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFileSystemUiCache uiCache,
        INavigator navigator,
        FileSystemSystemDbService fileSystemSystemDbService,
        HttpClient httpClient,
        IResponseHandler responseHandler
    )
    {
        httpClient.BaseAddress = new(options.Url);

        return new FilesUiService(
            new FileSystemHttpService(
                httpClient,
                new()
                {
                    TypeInfoResolver = AysJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                new TryPolicyService(
                    3,
                    TimeSpan.FromSeconds(1),
                    _ => appState.SetServiceMode(nameof(FilesUiService), ServiceMode.Offline)
                ),
                headersFactory
            ),
            fileSystemSystemDbService,
            appState,
            uiCache,
            navigator,
            nameof(FilesUiService),
            responseHandler
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

    public static FilesServiceOptions GetFilesServiceOptions(ISpravaConfig configuration)
    {
        return configuration.FilesService;
    }
}
