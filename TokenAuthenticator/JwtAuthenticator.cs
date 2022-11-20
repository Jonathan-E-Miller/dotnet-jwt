using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TokenAuthenticator;

public class JwtAuthenticator : ITokenAuthenticator
{
    private readonly IConfiguration _configuration;
    public JwtAuthenticator(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public string GeneratorToken()
    {
        var key = _configuration["Jwt:Key"] ?? throw new Exception("Invalid configuration");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));    
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
    
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],    
            _configuration["Jwt:Issuer"],    
            null,    
            expires: DateTime.Now.AddMinutes(30),    
            signingCredentials: credentials);    
    
        return new JwtSecurityTokenHandler().WriteToken(token);    
    }

    public string ValidateToken()
    {
        throw new NotImplementedException();
    }
}