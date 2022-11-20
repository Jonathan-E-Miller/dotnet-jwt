namespace JwtAuthenticator;

public interface ITokenAuthenticator
{
    string GeneratorToken();
    string ValidateToken();
}