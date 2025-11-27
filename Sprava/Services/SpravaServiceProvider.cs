using Cromwell.Services;
using Inanna.Services;
using Sprava.Ui;
using Jab;
using Manis.Contract;
using Manis.Contract.Services;
using Melnikov.Models;
using Melnikov.Services;
using Melnikov.Ui;

namespace Sprava.Services;

[ServiceProvider]
[Import(typeof(ICromwellServiceProvider))]
[Import(typeof(IMelnikovServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(IManisValidator), typeof(ManisValidator))]
[Singleton(typeof(NavigationBarViewModel))]
[Transient(typeof(CreateUserViewModel), Factory = nameof(GetCreateUserViewModel))]
[Transient(typeof(ManisServiceOptions), Factory = nameof(GetManisServiceOptions))]
public partial class SpravaServiceProvider : Inanna.Services.IServiceProvider
{
    public static ManisServiceOptions GetManisServiceOptions()
    {
        return new ManisServiceOptions
        {
            Url = "https://localhost:7027",
        };
    }

    public static CreateUserViewModel GetCreateUserViewModel(IApplicationResourceService appResourceService,
        IManisService manisService, IManisValidator manisValidator)
    {
        return new CreateUserViewModel(appResourceService.GetResource<string>("Lang.CreateUser"), manisService, manisValidator);
    }
}