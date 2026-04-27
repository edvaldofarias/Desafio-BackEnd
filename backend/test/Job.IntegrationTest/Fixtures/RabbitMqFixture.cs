using RabbitMQ.Client;

namespace Job.IntegrationTest.Fixtures;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    public string HostName { get; private set; } = "localhost";
    public int Port { get; private set; } = 5672;
    public string UserName { get; private set; } = "guest";
    public string Password { get; private set; } = "guest";

    public bool IsAvailable { get; private set; }
    public string? UnavailableReason { get; private set; }

    public Task InitializeAsync()
    {
        HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? HostName;
        Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var p) ? p : Port;
        UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? UserName;
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? Password;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = HostName,
                Port = Port,
                UserName = UserName,
                Password = Password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(3),
                SocketReadTimeout = TimeSpan.FromSeconds(3),
                SocketWriteTimeout = TimeSpan.FromSeconds(3),
            };

            using var connection = factory.CreateConnection("desafio-it-probe");
            using var channel = connection.CreateModel();

            IsAvailable = true;
        }
        catch (Exception ex)
        {
            IsAvailable = false;
            UnavailableReason = $"RabbitMQ indisponível em {HostName}:{Port} ({ex.GetType().Name}: {ex.Message})";
        }

        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
