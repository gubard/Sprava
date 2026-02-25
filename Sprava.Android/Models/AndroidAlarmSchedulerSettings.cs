using Gaia.Services;

namespace Sprava.Android.Models;

public sealed class AndroidAlarmSchedulerSettings
    : ObjectStorageValue<AndroidAlarmSchedulerSettings>
{
    public int[] RequestCodes { get; set; } = [];
}
