namespace Job.WebApi.Services;

public interface ITokenService
{
    string GenerateToken(string name, string role);
}
