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
    FilesServiceOptions FilesService { get; }
}

public class SpravaConfig : ISpravaConfig
{
    public SpravaConfig(Stream stream)
    {
        var options = JsonSerializer
            .Deserialize<SpravaOptions>(stream, SpravaJsonContext.Default.Options)
            .ThrowIfNull();

        AuthenticationService = options.AuthenticationService;
        CredentialService = options.CredentialService;
        ToDoService = options.ToDoService;
        FilesService = options.FilesService;
    }

    public AuthenticationServiceOptions AuthenticationService { get; }
    public CredentialServiceOptions CredentialService { get; }
    public ToDoServiceOptions ToDoService { get; }
    public FilesServiceOptions FilesService { get; }
}
