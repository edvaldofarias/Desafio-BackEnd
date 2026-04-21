using System.Text;
using System.Text.Json;
using Job.Application.Messaging;
using Job.Domain.Entities.Notification;
using Job.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Job.Infrastructure.Messaging;

public sealed class MotoCreatedConsumer : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<MotoCreatedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public MotoCreatedConsumer(
        IOptions<RabbitMqOptions> options,
        ILogger<MotoCreatedConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _options = options.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            EnsureConnection();
            StartConsumer(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao inicializar consumidor RabbitMQ");
        }

        return Task.CompletedTask;
    }

    private void EnsureConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.MotoCreatedExchange, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(_options.MotoCreatedQueue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_options.MotoCreatedQueue, _options.MotoCreatedExchange, routingKey: string.Empty);
    }

    private void StartConsumer(CancellationToken stoppingToken)
    {
        if (_channel is null) return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.Span);
                var @event = JsonSerializer.Deserialize<MotoCreatedEvent>(json);

                if (@event is null)
                {
                    _logger.LogWarning("Mensagem MotoCreated inválida e descartada");
                    _channel.BasicAck(args.DeliveryTag, false);
                    return;
                }

                if (@event.Year != 2024)
                {
                    _logger.LogInformation("Evento ignorado, ano diferente de 2024: {Year}", @event.Year);
                    _channel.BasicAck(args.DeliveryTag, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JobContext>();
                var notification = new MotoNotificationEntity(
                    @event.MotoId,
                    @event.Year,
                    @event.Model,
                    @event.Plate,
                    @event.OccurredAt);
                await context.MotoNotifications.AddAsync(notification, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);

                _channel.BasicAck(args.DeliveryTag, false);
                _logger.LogInformation("Notificação persistida para moto {MotoId}", @event.MotoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar evento MotoCreated");
                _channel.BasicNack(args.DeliveryTag, false, requeue: true);
            }
        };

        _channel.BasicConsume(_options.MotoCreatedQueue, autoAck: false, consumer);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao desligar consumidor RabbitMQ");
        }

        return base.StopAsync(cancellationToken);
    }
}
