namespace TokenAuthenticator;

public interface ITokenAuthenticator
{
    string GeneratorToken();
    string ValidateToken();
}