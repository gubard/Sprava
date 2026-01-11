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
                Classes = { "text-wrap" },
            },
            PropertyEmptyValidationError empty => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.PropertyEmptyValidationError"),
                    _appResourceService.GetResource<string>($"Lang.{empty.PropertyName}")
                ),
                Classes = { "text-wrap" },
            },
            NotFoundValidationError userNotFound => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotFound"),
                    userNotFound.Identity
                ),
                Classes = { "text-wrap" },
            },
            AlreadyExistsValidationError alreadyExists => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.AlreadyExists"),
                    alreadyExists.Identity
                ),
                Classes = { "text-wrap" },
            },
            InvalidPasswordValidationError => new()
            {
                Text = _appResourceService.GetResource<string>("Lang.InvalidPassword"),
                Classes = { "text-wrap" },
            },
            ExceptionsValidationError exceptions => new()
            {
                Text = string.Join(
                    Environment.NewLine,
                    exceptions
                        .Exceptions.Select(e => $"{e.Message}{Environment.NewLine}{e.StackTrace}")
                        .Distinct()
                ),
                Classes = { "text-wrap" },
            },
            PropertyInvalidValidationError propertyInvalid => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.InvalidValue"),
                    propertyInvalid.PropertyName
                ),
                Classes = { "text-wrap" },
            },
            PropertyMaxSizeValidationError propertyMaxSize => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.MaxSizeReached"),
                    propertyMaxSize
                ),
                Classes = { "text-wrap" },
            },
            PropertyMinSizeValidationError propertyMinSize => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.MinSizeNotReached"),
                    propertyMinSize
                ),
                Classes = { "text-wrap" },
            },
            PropertyNotEqualValidationError propertyNotEqualValidationError => new()
            {
                Text = _stringFormater.Format(
                    _appResourceService.GetResource<string>("Lang.NotEqualError"),
                    propertyNotEqualValidationError
                ),
                Classes = { "text-wrap" },
            },
            _ => new TextBlock
            {
                Text = $"Not found \"{param.GetType()}\"",
                Classes = { "text-wrap" },
            },
        };
    }

    public bool Match(object? data)
    {
        return data is ValidationError;
    }
}
