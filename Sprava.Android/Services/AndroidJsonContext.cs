using System.Text.Json.Serialization;
using Sprava.Android.Models;

namespace Sprava.Android.Services;

[JsonSerializable(typeof(AndroidAlarmSchedulerSettings))]
public sealed partial class AndroidJsonContext : JsonSerializerContext;
