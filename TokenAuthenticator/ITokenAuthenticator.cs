namespace TokenAuthenticator;

public interface ITokenAuthenticator
{
    string GeneratorToken(int userId);
    bool ValidateToken(string token);
}