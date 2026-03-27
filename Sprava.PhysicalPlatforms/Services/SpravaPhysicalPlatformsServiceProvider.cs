using Jab;
using Nestor.Db.LiteDb.Services;

namespace Sprava.PhysicalPlatforms.Services;

[ServiceProviderModule]
[Transient(typeof(IDatabaseFactory), typeof(DatabaseFactory))]
[Transient(typeof(IUltraLiteDatabaseFactory), typeof(FileUiUltraLiteDatabaseFactory))]
public interface ISpravaPhysicalPlatformsServiceProvider;
