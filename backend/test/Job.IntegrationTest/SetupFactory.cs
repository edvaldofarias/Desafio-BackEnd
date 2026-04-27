using Job.IntegrationTest.Fixtures;
using Job.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Job.IntegrationTest;

[Collection("Database")]
public class SetupFactory(DbFixture dbFixture, RabbitMqFixture rabbitFixture) : WebApplicationFactory<Program>
{
    public DbFixture Db => dbFixture;
    public RabbitMqFixture Rabbit => rabbitFixture;
    public string MotoCreatedQueue { get; } = $"moto.created.test-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", dbFixture.ConnectionString),
                new KeyValuePair<string, string?>("Jwt:Secret", "test-secret-key-with-at-least-32-characters!!"),
                new KeyValuePair<string, string?>("Storage:RootPath", Path.Combine(Path.GetTempPath(), "job-it-uploads")),
                new KeyValuePair<string, string?>("Storage:PublicBaseUrl", "/uploads"),
                new KeyValuePair<string, string?>("RabbitMq:HostName", rabbitFixture.HostName),
                new KeyValuePair<string, string?>("RabbitMq:Port", rabbitFixture.Port.ToString()),
                new KeyValuePair<string, string?>("RabbitMq:UserName", rabbitFixture.UserName),
                new KeyValuePair<string, string?>("RabbitMq:Password", rabbitFixture.Password),
                new KeyValuePair<string, string?>("RabbitMq:MotoCreatedExchange", "moto.created.test"),
                new KeyValuePair<string, string?>("RabbitMq:MotoCreatedQueue", MotoCreatedQueue),
            });
        });
    }
}
