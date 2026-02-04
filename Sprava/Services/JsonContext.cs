using System.Text.Json.Serialization;
using Cai.Models;
using Cromwell.Models;
using Diocles.Models;
using Melnikov.Models;
using Sprava.Models;

namespace Sprava.Services;

[JsonSerializable(typeof(AuthenticationServiceOptions))]
[JsonSerializable(typeof(CredentialServiceOptions))]
[JsonSerializable(typeof(ToDoServiceOptions))]
[JsonSerializable(typeof(SpravaOptions))]
[JsonSerializable(typeof(FileSystemServiceOptions))]
[JsonSerializable(typeof(FileStorageServiceOptions))]
public partial class OptionsJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(CromwellSettings))]
[JsonSerializable(typeof(AuthenticationSettings))]
[JsonSerializable(typeof(ToDosSetting))]
[JsonSerializable(typeof(SignInSettings))]
[JsonSerializable(typeof(ToDoParametersSettings))]
public partial class SettingsJsonContext : JsonSerializerContext;
