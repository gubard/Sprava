using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Sprava.Helpers;

public static class FileInfoExtension
{
    public static IDbConnectionFactory InitDbContext(this FileInfo file, IMigrator migrator)
    {
        var factory = new SqliteDbConnectionFactory(file);
        migrator.Migrate(factory);

        return factory;
    }
}
