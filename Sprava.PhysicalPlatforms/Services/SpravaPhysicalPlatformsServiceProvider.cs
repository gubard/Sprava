using Aya.Contract.Services;
using Aya.Db.Services;
using Cai.Services;
using Cromwell.Services;
using Diocles.Services;
using Gaia.Models;
using Gaia.Services;
using Hestia.Contract.Services;
using Hestia.Db.Services;
using Inanna.Models;
using Jab;
using Neotoma.Contract.Services;
using Neotoma.Db.Services;
using Nestor.Db.Services;
using Pheidippides.Services;
using Rooster.Contract.Services;
using Rooster.Db.Services;
using Sprava.Services;
using Turtle.Contract.Services;
using Turtle.Db.Services;
using Weber.Services;

namespace Sprava.PhysicalPlatforms.Services;

[ServiceProviderModule]
[Transient(typeof(IFileSystemDbCache), Factory = nameof(GetFileSystemDbService))]
[Transient(typeof(ICredentialDbCache), Factory = nameof(GetCredentialDbService))]
[Transient(typeof(IToDoDbCache), Factory = nameof(GetToDoDbService))]
[Transient(typeof(IFileStorageDbCache), Factory = nameof(GetFileStorageDbService))]
[Singleton(typeof(IAlarmDbCache), Factory = nameof(GetAlarmDbService))]
[Transient(typeof(IFileSystemDbService), Factory = nameof(GetFileSystemDbService))]
[Transient(typeof(ICredentialDbService), Factory = nameof(GetCredentialDbService))]
[Transient(typeof(IToDoDbService), Factory = nameof(GetToDoDbService))]
[Transient(typeof(IFileStorageDbService), Factory = nameof(GetFileStorageDbService))]
[Singleton(typeof(IAlarmDbService), Factory = nameof(GetAlarmDbService))]
[Transient(typeof(IObjectStorage), Factory = nameof(GetObjectStorage))]
public interface ISpravaPhysicalPlatformsServiceProvider
{
    public static IObjectStorage GetObjectStorage(
        IDbConnectionFactory factory,
        ISerializer serializer
    )
    {
        return new DbObjectStorage(factory, serializer);
    }

    public static AlarmAdoDbService GetAlarmDbService(
        AppState appState,
        IDbConnectionFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(AlarmUiService))
        );
    }
    public static FileStorageAdoDbService GetFileStorageDbService(
        AppState appState,
        IDbConnectionFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(FileStorageUiService))
        );
    }

    public static FileSystemAdoDbService GetFileSystemDbService(
        AppState appState,
        IDbConnectionFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(FileSystemUiService))
        );
    }
    public static CredentialAdoDbService GetCredentialDbService(
        AppState appState,
        IDbConnectionFactory factory,
        IFactory<DbValues> dbValues
    )
    {
        return new(
            factory,
            dbValues,
            new DbServiceOptionsUiFactory(appState, nameof(CredentialUiService))
        );
    }
    public static ToDoAdoDbService GetToDoDbService(
        AppState appState,
        ToDoParametersFillerService parametersFillerService,
        IDbConnectionFactory factory,
        IToDoValidator validator,
        IFactory<DbValues> dbValuesFactory
    )
    {
        return new(
            factory,
            dbValuesFactory,
            parametersFillerService,
            validator,
            new DbServiceOptionsUiFactory(appState, nameof(ToDoUiService))
        );
    }
}
