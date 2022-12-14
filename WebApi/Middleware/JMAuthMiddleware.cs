using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using TokenAuthenticator;

namespace WebApi.Middleware;

public class JMAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenAuthenticator _tokenAuthenticator;

    public JMAuthMiddleware(RequestDelegate next, ITokenAuthenticator authenticator)
    {
        _next = next;
        _tokenAuthenticator = authenticator;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<JMAuth>();
            if (attribute != null)
            {
                bool hasToken = context.Request.Headers.TryGetValue("jm-token", out StringValues token);
                
                if (hasToken && !string.IsNullOrEmpty(token.First()) && _tokenAuthenticator.ValidateToken(token.First()))
                {
                    await _next(context); // Here the action in the controller is called
                }
                else
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid token");
                }
            }
            else
            {
                await _next(context);   
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500; // Internal Server Error
            await context.Response.WriteAsync(ex.Message);
        }
    }
}