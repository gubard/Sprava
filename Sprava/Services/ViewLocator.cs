using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Inanna.Models;
using Inanna.Services;
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
            var control = builder.Invoke();

            if (param is IInit init)
            {
                control.Initialized += (_, _) => init.InitAsync(CancellationToken.None);
            }

            if (param is ILoad load)
            {
                control.Loaded += (_, _) => load.LoadAsync(CancellationToken.None);
            }

            return control;
        }

        return new TextBlock { Text = $"Not found \"{type}\"", Classes = { "plain-text" } };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
