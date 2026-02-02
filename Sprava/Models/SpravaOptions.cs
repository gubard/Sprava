using Cai.Models;
using Cromwell.Models;
using Diocles.Models;
using Melnikov.Models;

namespace Sprava.Models;

public class SpravaOptions
{
    public AuthenticationServiceOptions AuthenticationService { get; set; } = new();
    public CredentialServiceOptions CredentialService { get; set; } = new();
    public ToDoServiceOptions ToDoService { get; set; } = new();
    public FileSystemServiceOptions FileSystemService { get; set; } = new();
    public FileStorageServiceOptions FileStorageService { get; set; } = new();
}
