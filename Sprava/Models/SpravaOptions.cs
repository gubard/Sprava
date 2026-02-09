namespace Sprava.Models;

public sealed class SpravaOptions
{
    public AuthenticationServiceOptions AuthenticationService { get; set; } = new();
    public CredentialServiceOptions CredentialService { get; set; } = new();
    public ToDoServiceOptions ToDoService { get; set; } = new();
    public FileSystemServiceOptions FileSystemService { get; set; } = new();
    public FileStorageServiceOptions FileStorageService { get; set; } = new();
    public AlarmServiceOptions AlarmService { get; set; } = new();
}

public sealed class FileSystemServiceOptions
{
    public string Url { get; set; } = string.Empty;
}

public sealed class CredentialServiceOptions
{
    public string Url { get; set; } = string.Empty;
}

public sealed class AuthenticationServiceOptions
{
    public string Url { get; set; } = string.Empty;
}

public sealed class ToDoServiceOptions
{
    public string Url { get; set; } = string.Empty;
}

public sealed class AlarmServiceOptions
{
    public string Url { get; set; } = string.Empty;
}

public sealed class FileStorageServiceOptions
{
    public string Url { get; set; } = string.Empty;
}
