using Gaia.Models;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Melnikov.Services;
using Melnikov.Ui;

namespace Sprava.Services;

public sealed class ErrorDialogFactory : IErrorDialogFactory
{
    public ErrorDialogFactory(
        IInannaViewModelFactory viewModelFactory,
        IAppResourceService appResourceService,
        IDialogService dialogService,
        IAuthenticationUiService authenticationUiService
    )
    {
        _viewModelFactory = viewModelFactory;
        _appResourceService = appResourceService;

        _unauthorizedDialogButton = new(
            _appResourceService.GetResource<string>("Lang.Ok"),
            UiHelper.CreateCommand(async ct =>
            {
                await dialogService.CloseMessageBoxAsync(ct);
                await authenticationUiService.LogoutAsync(ct);
                await UiHelper.NavigateToAsync<SignInViewModel>(ct);
            }),
            null,
            DialogButtonType.Primary
        );
    }

    public DialogViewModel Create(Exception[] input)
    {
        return new(
            _appResourceService.GetResource<string>("Lang.Error").DispatchToDialogHeader(),
            _viewModelFactory.CreateException(input),
            UiHelper.OkButton
        );
    }

    public DialogViewModel Create(ValidationError[] input)
    {
        if (input.Any(x => x is UnauthorizedValidationError))
        {
            return new(
                _appResourceService.GetResource<string>("Lang.Error").DispatchToDialogHeader(),
                _viewModelFactory.CreateValidationErrors(input),
                _unauthorizedDialogButton
            );
        }

        return new(
            _appResourceService.GetResource<string>("Lang.Error").DispatchToDialogHeader(),
            _viewModelFactory.CreateValidationErrors(input),
            UiHelper.OkButton
        );
    }

    private readonly IInannaViewModelFactory _viewModelFactory;
    private readonly IAppResourceService _appResourceService;
    private readonly DialogButton _unauthorizedDialogButton;
}
