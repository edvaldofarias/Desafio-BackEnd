using System.Text;
using System.Text.Json;
using Job.Application.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Job.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly Lazy<IConnection> _connection;
    private readonly Lazy<IModel> _channel;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        _connection = new Lazy<IConnection>(() =>
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                DispatchConsumersAsync = true,
            };
            return factory.CreateConnection();
        });

        _channel = new Lazy<IModel>(() =>
        {
            var channel = _connection.Value.CreateModel();
            channel.ExchangeDeclare(_options.MotoCreatedExchange, ExchangeType.Fanout, durable: true);
            return channel;
        });
    }

    public Task PublishMotoCreatedAsync(MotoCreatedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var properties = _channel.Value.CreateBasicProperties();
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;
            properties.MessageId = Guid.NewGuid().ToString();

            _channel.Value.BasicPublish(
                exchange: _options.MotoCreatedExchange,
                routingKey: string.Empty,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Evento MotoCreated publicado para moto {MotoId}", @event.MotoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao publicar evento MotoCreated para moto {MotoId}", @event.MotoId);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_channel.IsValueCreated)
        {
            _channel.Value.Close();
            _channel.Value.Dispose();
        }

        if (_connection.IsValueCreated)
        {
            _connection.Value.Close();
            _connection.Value.Dispose();
        }
    }
}
