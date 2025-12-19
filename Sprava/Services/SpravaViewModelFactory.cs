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
    private readonly Application _application;
    private readonly IStorageService _storageService;

    public SpravaViewModelFactory(Application application, IStorageService storageService)
    {
        _application = application;
        _storageService = storageService;
    }

    public AppSettingViewModel Create()
    {
        return new(
            _application,
            DiHelper.ServiceProvider.GetService<ISettingsService<SpravaSettings>>(),
            _storageService
        );
    }
}
