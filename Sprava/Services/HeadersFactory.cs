using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Melnikov.Services;

namespace Sprava.Services;

public sealed class HeadersFactory : IFactory<Memory<HttpHeader>>
{
    private readonly IAuthenticationUiService _service;

    public HeadersFactory(IAuthenticationUiService service)
    {
        _service = service;
    }

    public Memory<HttpHeader> Create()
    {
        var token = _service.Token.ThrowIfNull();

        return new[]
        {
            new HttpHeader(HttpHeader.Authorization, $"{HttpHeader.Bearer} {token.Token}"),
            new HttpHeader(HttpHeader.TimeZoneOffset, DateTimeOffset.UtcNow.Offset.ToString()),
        };
    }
}
