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
            PropertyZeroValidationError zero => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyZeroValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{zero.PropertyName}")
                ),
                Classes = { "plain-text" },
            },
            PropertyEmptyValidationError empty => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyEmptyValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{empty.PropertyName}")
                ),
                Classes = { "plain-text" },
            },
            NotFoundValidationError userNotFound => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotFound"),
                    userNotFound.Identity
                ),
                Classes = { "plain-text" },
            },
            AlreadyExistsValidationError alreadyExists => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.AlreadyExists"),
                    alreadyExists.Identity
                ),
                Classes = { "plain-text" },
            },
            InvalidPasswordValidationError => new()
            {
                Text = _appResourceService.GetResource<string>("Lang.InvalidPassword"),
                Classes = { "plain-text" },
            },
            ExceptionsValidationError exceptions => new()
            {
                Text = string.Join(
                    Environment.NewLine,
                    exceptions
                        .Exceptions.Select(e => $"{e.Message}{Environment.NewLine}{e.StackTrace}")
                        .Distinct()
                ),
                Classes = { "plain-text" },
            },
            PropertyInvalidValidationError propertyInvalid => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.InvalidValue"),
                    propertyInvalid.PropertyName
                ),
                Classes = { "plain-text" },
            },
            PropertyMaxSizeValidationError propertyMaxSize => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.MaxSizeReached"),
                    propertyMaxSize
                ),
                Classes = { "plain-text" },
            },
            PropertyMinSizeValidationError propertyMinSize => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.MinSizeNotReached"),
                    propertyMinSize
                ),
                Classes = { "plain-text" },
            },
            PropertyNotEqualValidationError propertyNotEqualValidationError => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotEqualError"),
                    propertyNotEqualValidationError
                ),
                Classes = { "plain-text" },
            },
            _ => new TextBox
            {
                Text = $"Not found \"{param.GetType()}\"",
                Classes = { "plain-text" },
            },
        };
    }

    public bool Match(object? data)
    {
        return data is ValidationError;
    }
}
