using Avalonia.Media.Fonts;

namespace Sprava.Services;

public class JetBrainsMonoFontCollection : EmbeddedFontCollection
{
    public JetBrainsMonoFontCollection()
        : base(
            new("fonts:JetBrainsMono", UriKind.Absolute),
            new("avares://Sprava/Assets/Fonts/JetBrainsMono", UriKind.Absolute)
        ) { }
}
