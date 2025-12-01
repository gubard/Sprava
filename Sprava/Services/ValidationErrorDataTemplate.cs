using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Cromwell.Services;
using Gaia.Errors;
using Gaia.Helpers;
using Inanna.Services;

namespace Sprava.Services;

public class ValidationErrorDataTemplate : IDataTemplate
{
    private readonly IApplicationResourceService _appResourceService =
        DiHelper.ServiceProvider.GetService<IApplicationResourceService>();

    private readonly IStringFormater _stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();

    public Control? Build(object? param)
    {
        return param switch
        {
            null => null,
            PropertyZeroValidationError zero => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyZeroValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{zero.PropertyName}")),
            },
            PropertyEmptyValidationError empty => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyEmptyValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{empty.PropertyName}")),
            },
            NotFoundValidationError userNotFound => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotFound"),
                    userNotFound.Identity),
            },
            AlreadyExistsValidationError alreadyExists => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.AlreadyExists"),
                    alreadyExists.Identity),
            },
            _ => new TextBlock
            {
                Text = $"Not found \"{param.GetType()}\"",
            },
        };
    }

    public bool Match(object? data)
    {
        return data is ValidationError;
    }
}