using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SkillShare.Producer.Interfaces;

namespace SkillShare.Producer;

public class Producer : IMessageProducer
{
    public void SendMessage<T>(T message, string routingKey, string? exchange = default)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin",
            Port = 5672
        };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var json = JsonConvert.SerializeObject(message, Formatting.Indented,
        new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);
    }
}