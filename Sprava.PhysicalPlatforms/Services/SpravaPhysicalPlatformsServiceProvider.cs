using Jab;
using Nestor.Db.LiteDb.Services;

namespace Sprava.PhysicalPlatforms.Services;

[ServiceProviderModule]
[Singleton(typeof(IDatabaseFactory), typeof(UiDatabaseFactory))]
public interface ISpravaPhysicalPlatformsServiceProvider;
