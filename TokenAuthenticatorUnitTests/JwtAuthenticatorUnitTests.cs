using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using TokenAuthenticator;

namespace JwtAuthenticatorUnitTests;

public class JwtAuthenticatorUnitTests
{
    private JwtAuthenticator? _sut;
    private IConfiguration _configuration;
    
    [SetUp]
    public void Setup()
    {
        //Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Key", "ALongStringContainingEnoughCharacters"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _sut = new JwtAuthenticator(_configuration);
    }

    [SuppressMessage("Usage", "CS8625:Cannot convert null literal to non-nullable reference type", Justification = "Not production code.")]
    [Test]
    public void Constructor_WhenCalledWithoutIConfiguration_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _ = new JwtAuthenticator(null));
        Assert.That(exception!.ParamName, Is.EqualTo("configuration"));
    }
    
    [Test]
    public void Constructor_WhenCalledWithIConfiguration_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _ = new JwtAuthenticator(_configuration));
    }

    [Test]
    public void GenerateToken_WhenCalledWithValidConfiguration_GeneratesToken()
    {
        string token = _sut!.GeneratorToken();
        
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [Test]
    public void GenerateToken_WhenCalledWithInvalidConfiguration_ThrowsException()
    {

        var configuration = new ConfigurationBuilder()
            .Build();
        
        var sut = new JwtAuthenticator(configuration);

        var exception = Assert.Throws<Exception>(() => sut.GeneratorToken());
        
        Assert.That(exception!.Message, Is.EqualTo("Invalid configuration"));
    }
}