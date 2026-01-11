using Avalonia;
using Sprava.Services;

namespace Sprava.Helpers;

public static class AppBuilderExtension
{
    public static AppBuilder WithJetBrainsMonoFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
            fontManager.AddFontCollection(new JetBrainsMonoFontCollection())
        );
    }
}
