using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Melnikov.Services;

namespace Sprava.Services;

public class HeadersFactory : IFactory<Memory<HttpHeader>>
{
    private readonly IUiAuthenticationService _authenticationService;

    public HeadersFactory(IUiAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Memory<HttpHeader> Create()
    {
        var token = _authenticationService.Token.ThrowIfNull();

        return new[]
        {
            new HttpHeader("Authenticate", $"Bearer {token.Token}"),
        };
    }
}