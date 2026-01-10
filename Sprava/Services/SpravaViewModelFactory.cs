using Avalonia;
using Gaia.Services;
using Sprava.Ui;

namespace Sprava.Services;

public interface ISpravaViewModelFactory : IFactory<AppSettingViewModel>;

public class SpravaViewModelFactory : ISpravaViewModelFactory
{
    public SpravaViewModelFactory(Application application, IObjectStorage objectStorage)
    {
        _application = application;
        _objectStorage = objectStorage;
    }

    public AppSettingViewModel Create()
    {
        return new(_application, _objectStorage);
    }

    private readonly Application _application;
    private readonly IObjectStorage _objectStorage;
}
