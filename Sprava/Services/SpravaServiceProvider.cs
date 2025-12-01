using Cromwell.Services;
using Cromwell.Ui;
using Inanna.Helpers;
using Jab;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;
using Sprava.Ui;
using IServiceProvider = Inanna.Services.IServiceProvider;

namespace Sprava.Services;

[ServiceProvider]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(SignInViewModel), Factory = nameof(GetLoginViewModel))]
[Transient(typeof(IManisValidator), typeof(ManisValidator))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(SignUpViewModel))]
[Transient(typeof(ManisServiceOptions), Factory = nameof(GetManisServiceOptions))]
public partial class SpravaServiceProvider : IServiceProvider
{
    public static SignInViewModel GetLoginViewModel(IManisService manisService)
    {
        return new(manisService, UiHelper.NavigateToAsync<RootCredentialsViewModel>);
    }

    public static ManisServiceOptions GetManisServiceOptions()
    {
        return new()
        {
            Url = "https://localhost:7027",
        };
    }
}