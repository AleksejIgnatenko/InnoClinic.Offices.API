using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Infrastructure.Enums.Queues;
using InnoClinic.Offices.Infrastructure.Options.RabbitMQ;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace InnoClinic.Offices.Application.Services;

/// <summary>
/// Service for interacting with RabbitMQ for queue management and message publishing.
/// </summary>
public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQOptions _rabbitMqOptions;
    private readonly ConnectionFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQService"/> class.
    /// </summary>
    /// <param name="rabbitMqOptions">The RabbitMQ configuration options.</param>
    public RabbitMQService(IOptions<RabbitMQOptions> rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions.Value;

        _factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };
    }

    /// <summary>
    /// Creates the necessary queues for the offices.
    /// </summary>
    public async Task CreateQueuesAsync()
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();
        await Task.Run(() =>
        {
            channel.QueueDeclare(OfficeQueuesEnum.AddOffice.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(OfficeQueuesEnum.UpdateOffice.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(OfficeQueuesEnum.DeleteOffice.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
        });
    }

    /// <summary>
    /// Publishes a message to a specified queue.
    /// </summary>
    /// <param name="obj">The object to be published as a message.</param>
    /// <param name="queueName">The name of the queue to publish the message to.</param>
    public async Task PublishMessageAsync(object obj, string queueName)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var messageJson = JsonConvert.SerializeObject(obj);
        var body = Encoding.UTF8.GetBytes(messageJson);

        await Task.Run(() => channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body));
    }
}