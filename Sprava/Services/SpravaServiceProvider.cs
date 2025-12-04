using Cromwell.Models;
using Cromwell.Services;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Helpers;
using Jab;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Sprava.Services;

[ServiceProvider]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(ITryPolicyService), Factory = nameof(GetTryPolicyService))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetLoginViewModel))]
[Transient(typeof(IAuthenticationValidator), typeof(AuthenticationValidator))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(AuthenticationServiceOptions), Factory = nameof(GetAuthenticationServiceOptions))]
[Transient(typeof(CredentialServiceOptions), Factory = nameof(GetCredentialServiceOptions))]
public partial class SpravaServiceProvider : IServiceProvider
{
    public static SignInViewModel GetLoginViewModel(IUiAuthenticationService uiAuthenticationService)
    {
        return new(uiAuthenticationService, UiHelper.NavigateToAsync<RootCredentialsViewModel>, UiHelper.NavigateToAsync<RootCredentialsViewModel>);
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

    public static ITryPolicyService GetTryPolicyService()
    {
        return new TryPolicyService(3, TimeSpan.FromSeconds(1));
    }
}