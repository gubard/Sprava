using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Inanna.Models;
using Sprava.SourceGenerator;

namespace Sprava.Services;

public sealed class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var type = param.GetType();

        if (SpravaViewLocator.Builders.TryGetValue(type, out var builder))
        {
            return builder();
        }

        return new TextBlock { Text = $"Not found \"{type}\"", Classes = { "plain-text" } };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
