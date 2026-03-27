using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nestor.Db.LiteDb.Services;
using Sprava.Browser.Helpers;
using UltraLiteDB;

namespace Sprava.Browser.Services;

public sealed class BrowserDatabase : IDatabase
{
    private readonly MemoryStream _stream;
    private readonly UltraLiteDatabase _database;
    private readonly string _fileName;

    public BrowserDatabase(string fileName, UltraLiteDatabase database, MemoryStream stream)
    {
        _fileName = fileName;
        _database = database;
        _stream = stream;
    }

    public void Dispose()
    {
        _database.Dispose();
        _stream.Dispose();
    }

    public bool DropCollection(string name)
    {
        return _database.DropCollection(name);
    }

    public UltraLiteCollection<BsonDocument> GetCollection(string name, BsonAutoId autoId)
    {
        return _database.GetCollection(name, autoId);
    }

    public ConfiguredValueTaskAwaitable SaveChangesAsync(CancellationToken ct)
    {
        return SaveChangesCore(ct).ConfigureAwait(false);
    }

    public async ValueTask SaveChangesCore(CancellationToken ct)
    {
        var position = _stream.Position;
        _stream.Position = 0;
        await JsInterop.SaveDatabase(_fileName, _stream.ToArray());
        _stream.Position = position;
    }
}
