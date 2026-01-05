using Cromwell.Models;

namespace Sprava.Models;

public sealed class SpravaSettings
{
    public CromwellSettings CromwellSettings { get; set; } = new();
}
