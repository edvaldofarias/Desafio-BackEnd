namespace Job.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string MotoCreatedExchange { get; set; } = "moto.created";
    public string MotoCreatedQueue { get; set; } = "moto.created.year-2024";
}
