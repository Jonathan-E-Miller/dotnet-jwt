namespace TokenAuthenticator;

public interface ITokenAuthenticator
{
    string GenerateToken(int userId);
    bool ValidateToken(string token);
}