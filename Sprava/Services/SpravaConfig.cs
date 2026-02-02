using System.Text.Json;
using Cai.Models;
using Cromwell.Models;
using Diocles.Models;
using Gaia.Helpers;
using Melnikov.Models;
using Sprava.Models;

namespace Sprava.Services;

public interface ISpravaConfig
{
    AuthenticationServiceOptions AuthenticationService { get; }
    CredentialServiceOptions CredentialService { get; }
    ToDoServiceOptions ToDoService { get; }
    FileSystemServiceOptions FileSystemService { get; }
    FileStorageServiceOptions FileStorageService { get; }
}

public class SpravaConfig : ISpravaConfig
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
    }

    public AuthenticationServiceOptions AuthenticationService { get; }
    public CredentialServiceOptions CredentialService { get; }
    public ToDoServiceOptions ToDoService { get; }
    public FileSystemServiceOptions FileSystemService { get; }
    public FileStorageServiceOptions FileStorageService { get; }
}
