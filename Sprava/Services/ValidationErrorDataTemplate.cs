using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Services;
using Manis.Contract.Errors;

namespace Sprava.Services;

public class ValidationErrorDataTemplate : IDataTemplate
{
    private readonly IAppResourceService _appResourceService =
        DiHelper.ServiceProvider.GetService<IAppResourceService>();

    private readonly IStringFormater _stringFormater =
        DiHelper.ServiceProvider.GetService<IStringFormater>();

    public Control? Build(object? param)
    {
        return param switch
        {
            null => null,
            PropertyZeroValidationError zero => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyZeroValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{zero.PropertyName}")
                ),
            },
            PropertyEmptyValidationError empty => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyEmptyValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{empty.PropertyName}")
                ),
            },
            NotFoundValidationError userNotFound => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotFound"),
                    userNotFound.Identity
                ),
            },
            AlreadyExistsValidationError alreadyExists => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.AlreadyExists"),
                    alreadyExists.Identity
                ),
            },
            InvalidPasswordValidationError => new TextBox
            {
                Text = _appResourceService.GetResource<string>("Lang.InvalidPassword"),
            },
            ExceptionsValidationError exceptions => new TextBox
            {
                Text = string.Join(
                    Environment.NewLine,
                    exceptions
                        .Exceptions.Select(e => $"{e.Message}{Environment.NewLine}{e.StackTrace}")
                        .Distinct()
                ),
                IsReadOnly = true,
            },
            PropertyInvalidValidationError propertyInvalid => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.InvalidValue"),
                    propertyInvalid.PropertyName
                ),
            },
            PropertyMaxSizeValidationError propertyMaxSize => new TextBox
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.MaxSizeReached"),
                    propertyMaxSize
                ),
            },
            _ => new TextBlock { Text = $"Not found \"{param.GetType()}\"" },
        };
    }

    public bool Match(object? data)
    {
        return data is ValidationError;
    }
}
