using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Job.IntegrationTest;

internal static class TestTokens
{
    public const string Secret = "test-secret-key-with-at-least-32-characters!!";

    public static string Generate(string name, string role)
    {
        var handler = new JwtSecurityTokenHandler();
        handler.OutboundClaimTypeMap.Clear();
        var key = Encoding.UTF8.GetBytes(Secret);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public static HttpClient WithAdminAuth(this HttpClient client, string name = "admin@admin.com")
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Generate(name, "admin"));
        return client;
    }

    public static HttpClient WithMotoboyAuth(this HttpClient client, string cnpj = "00000000000000")
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Generate(cnpj, "entregador"));
        return client;
    }
}
