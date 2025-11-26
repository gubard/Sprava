using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Cromwell.SourceGenerator;
using Inanna.Models;
using Sprava.SourceGenerator;
using Melnikov.SourceGenerator;

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

        if (CromwellViewLocator.Builders.TryGetValue(type, out var builder))
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

        return new TextBlock
        {
            Text = $"Not found \"{type}\""
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}