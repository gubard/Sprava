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
    public ResponseHandler(IAuthenticationUiService authenticationUiService)
    {
        _authenticationUiService = authenticationUiService;
    }

    public ConfiguredValueTaskAwaitable HandleResponseAsync<TResponse>(
        TResponse response,
        CancellationToken ct
    )
        where TResponse : IResponse
    {
        return HandleResponseCore(response, ct).ConfigureAwait(false);
    }

    private readonly IAuthenticationUiService _authenticationUiService;

    private async ValueTask HandleResponseCore<TResponse>(TResponse response, CancellationToken ct)
        where TResponse : IResponse
    {
        if (!response.ValidationErrors.OfType<UnauthorizedValidationError>().Any())
        {
            return;
        }

        await _authenticationUiService.LogoutAsync(ct);
        await UiHelper.NavigateToAsync<SignInViewModel>(ct);
    }
}
