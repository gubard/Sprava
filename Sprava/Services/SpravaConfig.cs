using System.Text.Json;
using Gaia.Helpers;
using Sprava.Models;

namespace Sprava.Services;

public interface ISpravaConfig
{
    AuthenticationServiceOptions AuthenticationService { get; }
    CredentialServiceOptions CredentialService { get; }
    ToDoServiceOptions ToDoService { get; }
    FileSystemServiceOptions FileSystemService { get; }
    FileStorageServiceOptions FileStorageService { get; }
    AlarmServiceOptions AlarmService { get; }
    IEnumerable<DownloadInstallItem> Downloads { get; }
}

public sealed class SpravaConfig : ISpravaConfig
{
    public SpravaConfig(Stream stream)
    {
        var options = JsonSerializer
            .Deserialize<SpravaOptions>(stream, OptionsJsonContext.Default.Options)
            .ThrowIfNull();

        AuthenticationService = options.AuthenticationService;
        CredentialService = options.CredentialService;
        ToDoService = options.ToDoService;
        FileSystemService = options.FileSystemService;
        FileStorageService = options.FileStorageService;
        AlarmService = options.AlarmService;

        Downloads = options
            .Downloads.Select(x => new DownloadInstallItem(x.Name, x.Url.ToUri()))
            .ToArray();
    }

    public AuthenticationServiceOptions AuthenticationService { get; }
    public CredentialServiceOptions CredentialService { get; }
    public ToDoServiceOptions ToDoService { get; }
    public FileSystemServiceOptions FileSystemService { get; }
    public FileStorageServiceOptions FileStorageService { get; }
    public AlarmServiceOptions AlarmService { get; }
    public IEnumerable<DownloadInstallItem> Downloads { get; }
}
