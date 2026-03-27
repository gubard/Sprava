using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Sprava.Browser.Helpers;

[SupportedOSPlatform("browser")]
public static partial class JsInterop
{
    [JSImport("windowOpen", "window.js")]
    public static partial void WindowOpen(string url);

    [JSImport("getCurrentUrl", "window.js")]
    public static partial string? GetCurrentUrl();

    [JSImport("saveDatabase", "db-storage.js")]
    public static partial Task SaveDatabase(string fileName, byte[] data);

    [JSImport("loadDatabase", "db-storage.js")]
    public static partial Task<string?> LoadDatabase(string fileName);
}
