using Avalonia;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Services;
using Sprava.Models;
using Sprava.Ui;

namespace Sprava.Services;

public interface ISpravaViewModelFactory : IFactory<AppSettingViewModel>;

public class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(Application application)
    {
        _application = application;
    }

    public AppSettingViewModel Create()
    {
        return new(
            _application,
            DiHelper.ServiceProvider.GetService<ISettingsService<SpravaSettings>>()
        );
    }

    private readonly Application _application;
}
