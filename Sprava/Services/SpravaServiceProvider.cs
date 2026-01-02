using System.IdentityModel.Tokens.Jwt;
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
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Jab;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Nestor.Db.Sqlite.Helpers;
using Sprava.Models;
using Sprava.Ui;
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
public interface ISpravaServiceProvider : IServiceProvider
{
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
        IStorageService storageService
    )
    {
        return new(
            new FileInfo(
                $"{storageService.GetAppDirectory()}/settings/{appState.User.ThrowIfNull().Id}.db"
            ).InitDbContext()
        );
    }

    public static ISettingsService<MelnikovSettings> GetMelnikovSettingsService(
        IStorageService storageService
    )
    {
        return new MelnikovSettingsSettingsService(
            new FileInfo($"{storageService.GetAppDirectory()}/sprava.db").InitDbContext()
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
