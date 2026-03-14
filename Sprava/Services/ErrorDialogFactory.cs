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
    public ErrorDialogFactory(Gaia.Services.IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _unauthorizedDialogButton = new Lazy<DialogButton>(() =>
            new(
                _serviceProvider.GetService<IAppResourceService>().GetResource<string>("Lang.Ok"),
                _serviceProvider
                    .GetService<ICommandFactory>()
                    .CreateCommand(async ct =>
                    {
                        await _serviceProvider
                            .GetService<IDialogService>()
                            .CloseMessageBoxAsync(ct);
                        await _serviceProvider
                            .GetService<IAuthenticationUiService>()
                            .LogoutAsync(ct);
                        await _serviceProvider
                            .GetService<INavigator>()
                            .NavigateToAsync<SignInViewModel>(serviceProvider, ct);
                    }),
                null,
                DialogButtonType.Primary
            )
        );
    }

    public DialogViewModel Create(Exception[] input)
    {
        return new(
            _serviceProvider
                .GetService<IAppResourceService>()
                .GetResource<string>("Lang.Error")
                .DispatchToDialogHeader(),
            _serviceProvider.GetService<IInannaViewModelFactory>().CreateException(input),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<IDialogService>().OkButton
        );
    }

    public DialogViewModel Create(ValidationError[] input)
    {
        if (input.Any(x => x is UnauthorizedValidationError))
        {
            return new(
                _serviceProvider
                    .GetService<IAppResourceService>()
                    .GetResource<string>("Lang.Error")
                    .DispatchToDialogHeader(),
                _serviceProvider
                    .GetService<IInannaViewModelFactory>()
                    .CreateValidationErrors(input),
                _serviceProvider.GetService<ISafeExecuteWrapper>(),
                _unauthorizedDialogButton.Value
            );
        }

        return new(
            _serviceProvider
                .GetService<IAppResourceService>()
                .GetResource<string>("Lang.Error")
                .DispatchToDialogHeader(),
            _serviceProvider.GetService<IInannaViewModelFactory>().CreateValidationErrors(input),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<IDialogService>().OkButton
        );
    }

    private readonly Gaia.Services.IServiceProvider _serviceProvider;
    private readonly Lazy<DialogButton> _unauthorizedDialogButton;
}
