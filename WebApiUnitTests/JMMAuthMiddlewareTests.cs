using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Moq;
using TokenAuthenticator;
using WebApi;
using WebApi.Middleware;

namespace WebApiUnitTests;

public class JMAuthMiddlewareTests
{
    private const string ExpectedOutput = "Request handed over to next request delegate";
    private Mock<ITokenAuthenticator>? _tokenAuthenticatorMock;
    [SetUp]
    public void Setup()
    {
        _tokenAuthenticatorMock = new Mock<ITokenAuthenticator>();
    }

    [Test]
    public async Task Invoke_AuthRequiredTokenIsValid_ExecutesDelegate()
    {
        var sut = new JMAuthMiddleware(DummyNext, _tokenAuthenticatorMock!.Object);
        var httpContext = BuildHttpContext();
        _tokenAuthenticatorMock.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(true);
        
        await sut.Invoke(httpContext);
        
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        Assert.That(body, Is.EqualTo(ExpectedOutput));
    }

    [Test]
    public async Task Invoke_WhenAuthNotRequired_ExecutesDelegate()
    {
        var sut = new JMAuthMiddleware(DummyNext, _tokenAuthenticatorMock!.Object);
        var httpContext = BuildHttpContext(authRequired:false);

        await sut.Invoke(httpContext);
        
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        Assert.That(body, Is.EqualTo(ExpectedOutput));
    }

    [Test]
    public async Task Invoke_WhenAuthRequiredAndTokenMissing_ReturnsUnAuthorizedStatusCode()
    {
        var sut = new JMAuthMiddleware(DummyNext, _tokenAuthenticatorMock!.Object);
        var httpContext = BuildHttpContext();
        httpContext.Request.Headers.Clear();

        await sut.Invoke(httpContext);
        const int expected = 401;
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(expected));
    }

    [Test]
    public async Task Invoke_WhenAuthRequiredAndTokenIsInvalid_ReturnsUnAuthorizedStatusCode()
    {
        _tokenAuthenticatorMock!.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(false);
        
        var sut = new JMAuthMiddleware(DummyNext, _tokenAuthenticatorMock.Object);
        var httpContext = BuildHttpContext();
        await sut.Invoke(httpContext);
        const int expected = 401;
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(expected));
    }

    private static async Task DummyNext(HttpContext context)
    {
        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ExpectedOutput));
    }

    private static HttpContext BuildHttpContext(bool authRequired = true)
    {
        var context = new DefaultHttpContext()
        {
            Response = { Body = new MemoryStream() },
            Request = { Headers = { new KeyValuePair<string, StringValues>("jm-token", "token") }}
        };

        EndpointMetadataCollection collection;
        if (authRequired)
        {
            collection = new EndpointMetadataCollection(new List<Attribute>()
            {
                new JMAuth()
            });
        }
        else
        {
            collection = new EndpointMetadataCollection(new List<Attribute>());
        }

        Endpoint endpoint = new Endpoint(DummyNext, collection, "test");
        
        var ep = new EndpointFeature()
        {
            Endpoint = endpoint
        };
        
        context.Features.Set<IEndpointFeature>(ep);
        return context;
    }
}

public class EndpointFeature : IEndpointFeature
{
    public Endpoint? Endpoint { get; set; }
}