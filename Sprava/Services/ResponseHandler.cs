using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Helpers;
using Inanna.Services;
using Melnikov.Services;
using Melnikov.Ui;
using Nestor.Db.Models;

namespace Sprava.Services;

public sealed class ResponseHandler : IResponseHandler
{
    public ResponseHandler(IUiAuthenticationService uiAuthenticationService)
    {
        _uiAuthenticationService = uiAuthenticationService;
    }

    public ConfiguredValueTaskAwaitable HandleResponseAsync<TResponse>(
        TResponse response,
        CancellationToken ct
    )
        where TResponse : IResponse
    {
        return HandleResponseCore(response, ct).ConfigureAwait(false);
    }

    private readonly IUiAuthenticationService _uiAuthenticationService;

    private async ValueTask HandleResponseCore<TResponse>(TResponse response, CancellationToken ct)
        where TResponse : IResponse
    {
        if (!response.ValidationErrors.OfType<UnauthorizedValidationError>().Any())
        {
            return;
        }

        await _uiAuthenticationService.LogoutAsync(ct);
        await UiHelper.NavigateToAsync<SignInViewModel>(ct);
    }
}
