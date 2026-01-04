using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
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
using Nestor.Db.Services;
using Nestor.Db.Sqlite.Helpers;
using Sprava.Helpers;
using Sprava.Models;
using Sprava.Ui;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

[ServiceProviderModule]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Import(typeof(ICaiServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(AppState))]
[Singleton(typeof(ITryPolicyService), Factory = nameof(GetTryPolicyService))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(ISettingsService<CromwellSettings>), Factory = nameof(GetSettingsService))]
[Transient(typeof(ISettingsService<SpravaSettings>), Factory = nameof(GetSettingsService))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(
    typeof(ISettingsService<MelnikovSettings>),
    Factory = nameof(GetMelnikovSettingsService)
)]
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
public interface ISpravaServiceProvider : IServiceProvider
{
    public static IUiToDoService GetUiToDoService(
        ToDoServiceOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        ToDoParametersFillerService toDoParametersFillerService,
        IToDoCache toDoCache,
        INavigator navigator,
        IStorageService storageService,
        IToDoValidator toDoValidator,
        IMigrator migrator
    )
    {
        var user = appState.User.ThrowIfNull();

        return new UiToDoService(
            new HttpToDoService(
                new() { BaseAddress = new(options.Url) },
                new()
                {
                    TypeInfoResolver = HestiaJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                tryPolicyService,
                headersFactory
            ),
            new EfToDoService(
                new FileInfo($"{storageService.GetAppDirectory()}/{user.Id}.db").InitDbContext(
                    migrator
                ),
                new(DateTimeOffset.UtcNow.Offset, user.Id),
                toDoParametersFillerService,
                toDoValidator
            ),
            appState,
            toDoCache,
            navigator
        );
    }

    public static IUiCredentialService GetUiCredentialService(
        CredentialServiceOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        ICredentialCache cache,
        INavigator navigator,
        IStorageService storageService,
        GaiaValues gaiaValues,
        IMigrator migrator
    )
    {
        return new UiCredentialService(
            new HttpCredentialService(
                new() { BaseAddress = new(options.Url) },
                new()
                {
                    TypeInfoResolver = TurtleJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                tryPolicyService,
                headersFactory
            ),
            new EfCredentialService(
                new FileInfo(
                    $"{storageService.GetAppDirectory()}/{appState.User.ThrowIfNull().Id}.db"
                ).InitDbContext(migrator),
                gaiaValues
            ),
            appState,
            cache,
            navigator
        );
    }

    public static IUiFilesService GetUiFilesService(
        FilesServiceOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFilesCache toDoCache,
        INavigator navigator,
        IStorageService storageService,
        IMigrator migrator
    )
    {
        var user = appState.User.ThrowIfNull();

        return new UiFilesService(
            new HttpFilesService(
                new() { BaseAddress = new(options.Url) },
                new()
                {
                    TypeInfoResolver = AysJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                tryPolicyService,
                headersFactory
            ),
            new EfFilesService(
                new FileInfo($"{storageService.GetAppDirectory()}/{user.Id}.db").InitDbContext(
                    migrator
                ),
                new(DateTimeOffset.UtcNow.Offset, user.Id)
            ),
            appState,
            toDoCache,
            navigator
        );
    }

    public static IMigrator GetMigrator()
    {
        return new Migrator(SqliteMigration.Migrations);
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

    public static SettingsService GetSettingsService(
        AppState appState,
        IStorageService storageService,
        IMigrator migrator
    )
    {
        return new(
            new FileInfo(
                $"{storageService.GetAppDirectory()}/{appState.User.ThrowIfNull().Id}.db"
            ).InitDbContext(migrator)
        );
    }

    public static ISettingsService<MelnikovSettings> GetMelnikovSettingsService(
        IStorageService storageService,
        IMigrator migrator
    )
    {
        return new MelnikovSettingsSettingsService(
            new FileInfo($"{storageService.GetAppDirectory()}/sprava.db").InitDbContext(migrator)
        );
    }

    public static SignInViewModel GetSignInViewModel(
        IUiAuthenticationService uiAuthenticationService,
        ISettingsService<MelnikovSettings> settingsService,
        AppState appState
    )
    {
        return new(
            uiAuthenticationService,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            settingsService,
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

    public static ITryPolicyService GetTryPolicyService()
    {
        return new TryPolicyService(3, TimeSpan.FromSeconds(1));
    }
}
