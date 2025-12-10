using System.IdentityModel.Tokens.Jwt;
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

[ServiceProvider]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Import(typeof(IDioclesServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(AppState))]
[Singleton(typeof(ITryPolicyService), Factory = nameof(GetTryPolicyService))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(ISettingsService<CromwellSettings>),
    Factory = nameof(GetSettingsService))]
[Transient(typeof(ISettingsService<SpravaSettings>),
    Factory = nameof(GetSettingsService))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetSignInViewModel))]
[Transient(typeof(ISettingsService<MelnikovSettings>),
    Factory = nameof(GetMelnikovSettingsService))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(JwtSecurityTokenHandler))]
[Transient(typeof(IFactory<Memory<HttpHeader>>), typeof(HeadersFactory))]
[Transient(typeof(AuthenticationServiceOptions),
    Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions),
    Factory = nameof(GetCredentialServiceOptions))]
[Transient(typeof(ToDoServiceOptions), Factory = nameof(GetToDoServiceOptions))]
public partial class SpravaServiceProvider : IServiceProvider
{
    public static SettingsService GetSettingsService(AppState appState, IStorageService storageService)
    {
        return new(
            new FileInfo(
                    $"{storageService.GetAppDirectory()}/settings/{appState.User.ThrowIfNull().Id}.db")
               .InitDbContext());
    }

    public static ISettingsService<MelnikovSettings>
        GetMelnikovSettingsService(IStorageService storageService)
    {
        return new MelnikovSettingsSettingsService(
            new FileInfo($"{storageService.GetAppDirectory()}/sprava.db").InitDbContext());
    }

    public static SignInViewModel GetSignInViewModel(
        IUiAuthenticationService uiAuthenticationService,
        ISettingsService<MelnikovSettings> settingsService)
    {
        return new(uiAuthenticationService,
            UiHelper.NavigateToAsync<RootToDosViewModel>,
            UiHelper.NavigateToAsync<RootToDosViewModel>, settingsService);
    }

    public static AuthenticationServiceOptions GetAuthenticationServiceOptions()
    {
        return new()
        {
            Url = "https://localhost:7027",
        };
    }

    public static CredentialServiceOptions GetCredentialServiceOptions()
    {
        return new()
        {
            Url = "https://localhost:7089",
        };
    }

    public static ToDoServiceOptions GetToDoServiceOptions()
    {
        return new()
        {
            Url = "https://localhost:7089",
        };
    }

    public static ITryPolicyService GetTryPolicyService()
    {
        return new TryPolicyService(3, TimeSpan.FromSeconds(1));
    }

    public object GetService(Type type)
    {
        return ((System.IServiceProvider)this).GetService(type).ThrowIfNull();
    }
}