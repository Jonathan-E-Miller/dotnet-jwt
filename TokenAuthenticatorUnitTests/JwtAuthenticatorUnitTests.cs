using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
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
            {"Jwt:Key", "ALongStringContainingEnoughCharacters"},
            {"Jwt:Issuer", "TestAudience"}
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
    public void Constructor_WhenCalledWithMissingJwtKey_ThrowsInvalidConfigurationException()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Issuer", "TestAudience"}
        };
        var configuration = new ConfigurationManager()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var exception = Assert.Throws<InvalidConfigurationException>(() => _ = new JwtAuthenticator(configuration));
        
        Assert.That(exception!.Message, Is.EqualTo("Invalid configuration"));
    }
    
    [Test]
    public void Constructor_WhenCalledWithMissingJwtIssuer_ThrowsInvalidConfigurationException()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Key", "TestAudience"}
        };
        var configuration = new ConfigurationManager()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        var exception = Assert.Throws<InvalidConfigurationException>(() => _ = new JwtAuthenticator(configuration));
        
        Assert.That(exception!.Message, Is.EqualTo("Invalid configuration"));
    }
    
    [Test]
    public void Constructor_WhenCalledWithIConfiguration_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _ = new JwtAuthenticator(_configuration));
    }

    [Test]
    public void GenerateToken_WhenCalledWithValidConfiguration_GeneratesToken()
    {
        var token = _sut!.GeneratorToken(5);
        
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [Test]
    public void ValidateToken_WhenCalledWithValidToken_ReturnsTrue()
    {
        var token = _sut!.GeneratorToken(5);
        var result = _sut.ValidateToken(token);
        
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateToken_WhenCalledWithInvalidToken_ReturnsFalse()
    {
        var invalidToken = "invalidTestToken";
        var result = _sut.ValidateToken(invalidToken);
        Assert.IsFalse(result);

    }
}