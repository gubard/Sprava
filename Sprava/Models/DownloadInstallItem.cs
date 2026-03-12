namespace Sprava.Models;

public sealed class DownloadInstallItem
{
    public DownloadInstallItem(string name, Uri url)
    {
        Name = name;
        Url = url;
    }

    public string Name { get; }
    public Uri Url { get; }
}
