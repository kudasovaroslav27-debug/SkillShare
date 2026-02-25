

using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SkillShare.Domain.Settings;

namespace SkillShare.Consumer;
public class RabbitMqListener : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqSettings _settings;

    public RabbitMqListener(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;

        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName ?? "admin",
            Password = _settings.Password ?? "admin",
            Port = 5672,
            AutomaticRecoveryEnabled = true,
            RequestedHeartbeat = TimeSpan.FromSeconds(60)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (obj, basicDeliver) =>
        {
            var content = Encoding.UTF8.GetString(basicDeliver.Body.ToArray());
            Debug.WriteLine($"[RabbitMQ] Получено сообщение: {content}");

            _channel.BasicAck(basicDeliver.DeliveryTag, false);
        };

        _channel.BasicConsume(_settings.QueueName, false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen) _channel.Close();
        if (_connection.IsOpen) _connection.Close();
        base.Dispose();
    }
}