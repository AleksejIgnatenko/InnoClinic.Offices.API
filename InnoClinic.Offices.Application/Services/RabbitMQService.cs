using InnoClinic.Offices.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace InnoClinic.Offices.Application.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        public RabbitMQService(IOptions<RabbitMQSetting> rabbitMqSetting)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
        }

        public async Task PublishMessageAsync(object obj, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(obj);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await Task.Run(() => channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body));
        }
    }
}
