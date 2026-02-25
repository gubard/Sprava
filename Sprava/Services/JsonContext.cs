using System.Text.Json.Serialization;
using Cromwell.Models;
using Diocles.Models;
using Inanna.Models;
using Melnikov.Models;
using Pheidippides.Models;
using Sprava.Models;

namespace Sprava.Services;

[JsonSerializable(typeof(AuthenticationServiceOptions))]
[JsonSerializable(typeof(CredentialServiceOptions))]
[JsonSerializable(typeof(ToDoServiceOptions))]
[JsonSerializable(typeof(SpravaOptions))]
[JsonSerializable(typeof(FileSystemServiceOptions))]
[JsonSerializable(typeof(FileStorageServiceOptions))]
public sealed partial class OptionsJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(CromwellSettings))]
[JsonSerializable(typeof(AuthenticationSettings))]
[JsonSerializable(typeof(ToDosSetting))]
[JsonSerializable(typeof(ToDoParametersSettings))]
[JsonSerializable(typeof(SearchToDoSettings))]
[JsonSerializable(typeof(InannaSettings))]
[JsonSerializable(typeof(AlarmsSettings))]
public sealed partial class SettingsJsonContext : JsonSerializerContext;
