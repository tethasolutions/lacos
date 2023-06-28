using Lacos.GestioneCommesse.Application.Session;

namespace Lacos.GestioneCommesse.WebApi.Auth;

public class AccessTokenProvider : IAccessTokenProvider
{
    private const string AuthorizationHeaderKey = "Authorization";
    private const string AuthorizationQueryStringKey = "access_token";
    private const string AuthorizationType = "Bearer";

    private readonly IHttpContextAccessor httpContextAccessor;

    private string? accessToken;
    public string? AccessToken => accessToken ??= GetAccessTokenFromHeader() ?? GetAccessTokenFromQueryString();

    public AccessTokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    private string? GetAccessTokenFromHeader()
    {
        var headers = httpContextAccessor.HttpContext?.Request.Headers;

        if (headers == null || !headers.ContainsKey(AuthorizationHeaderKey))
        {
            return null;
        }

        var header = headers[AuthorizationHeaderKey];

        if (header.Count != 1)
        {
            return null;
        }

        var value = header[0];

        if (value == null || !value.StartsWith($"{AuthorizationType} "))
        {
            return null;
        }

        return value[(AuthorizationType.Length + 1)..];
    }

    private string? GetAccessTokenFromQueryString()
    {
        var query = httpContextAccessor.HttpContext?.Request.Query;

        if (query == null || !query.ContainsKey(AuthorizationQueryStringKey))
        {
            return null;
        }

        var values = query[AuthorizationQueryStringKey];

        if (values.Count != 1)
        {
            return null;
        }

        var value = values.Single();

        return value;
    }
}