using Cromwell.Services;
using Sprava.Ui;
using Jab;

namespace Sprava.Services;

[ServiceProvider]
[Import(typeof(ICromwellServiceProvider))]
[Singleton(typeof(MainViewModel))]
[Transient(typeof(PaneViewModel))]
[Singleton(typeof(NavigationBarViewModel))]
public partial class SpravaServiceProvider : Inanna.Services.IServiceProvider;