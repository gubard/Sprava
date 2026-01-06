using Microsoft.EntityFrameworkCore;
using Nestor.Db.Sqlite.Services;
using Sprava.Services;

namespace Sprava.Helpers;

public static class FileInfoExtension
{
    public static SpravaDbContext InitDbContext(this FileInfo file, IMigrator migrator)
    {
        var options = new DbContextOptionsBuilder<SpravaDbContext>()
            .UseSqlite($"Data Source={file}")
            .Options;

        var context = new SpravaDbContext(options);
        migrator.Migrate(context);

        return context;
    }
}
