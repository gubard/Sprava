using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Cromwell.SourceGenerator;
using Diocles.SourceGenerator;
using Inanna.Models;
using Inanna.SourceGenerator;
using Melnikov.SourceGenerator;
using Sprava.SourceGenerator;

namespace Sprava.Services;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var type = param.GetType();

        if (DioclesViewLocator.Builders.TryGetValue(type, out var builder))
        {
            return builder();
        }

        if (InannaViewLocator.Builders.TryGetValue(type, out builder))
        {
            return builder();
        }

        if (CromwellViewLocator.Builders.TryGetValue(type, out builder))
        {
            return builder();
        }

        if (SpravaViewLocator.Builders.TryGetValue(type, out builder))
        {
            return builder();
        }

        if (MelnikovViewLocator.Builders.TryGetValue(type, out builder))
        {
            return builder();
        }

        return new TextBlock { Text = $"Not found \"{type}\"" };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
