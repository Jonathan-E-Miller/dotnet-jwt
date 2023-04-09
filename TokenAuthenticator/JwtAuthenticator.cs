using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TokenAuthenticator;

public class JwtAuthenticator : ITokenAuthenticator
{
    private readonly IConfiguration _configuration;
    private readonly string _key;
    private readonly string _issuer;
    
    public JwtAuthenticator(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _key = _configuration["Jwt:Key"] ?? throw new InvalidConfigurationException("Invalid configuration");
        _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidConfigurationException("Invalid configuration");
    }
    
    public string GenerateToken(int userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));    
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
    
        var claims = new[] {
            new Claim("userId", userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        };    
        
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],    
            audience:_configuration["Jwt:Issuer"],    
            claims:claims,    
            expires: DateTime.Now.AddMinutes(30),    
            signingCredentials: credentials);    
    
        return new JwtSecurityTokenHandler().WriteToken(token);    
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = _issuer,
                    ValidAudiences = new[] { _issuer },
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true
                };

            var handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken(token, tokenValidationParameters, out _);
            return user != null;
        }
        catch (Exception)
        {
            // Something went wrong verifying the token
            return false;
        }
    }
}