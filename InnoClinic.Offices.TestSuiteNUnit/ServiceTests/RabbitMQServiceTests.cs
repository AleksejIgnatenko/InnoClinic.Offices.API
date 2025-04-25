using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Infrastructure.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Testcontainers.RabbitMq;

namespace InnoClinic.Offices.TestSuiteNUnit.ServiceTests;

class RabbitMQServiceTests
{
    private RabbitMqContainer _rabbitMqContainer;
    private IRabbitMQService _rabbitMQService;
    private RabbitMQOptions rabbitMQOptions;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.11-management")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5672)
            .Build();

        await _rabbitMqContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _rabbitMqContainer.DisposeAsync();
    }

    [SetUp]
    public void SetUp()
    {
        rabbitMQOptions = new RabbitMQOptions
        {
            HostName = _rabbitMqContainer.Hostname,
            UserName = "guest",
            Password = "guest",
        };

        var services = new ServiceCollection();
        services.Configure<RabbitMQOptions>(options =>
        {
            options.HostName = rabbitMQOptions.HostName;
            options.UserName = rabbitMQOptions.UserName;
            options.Password = rabbitMQOptions.Password;
        });
        services.AddSingleton<IRabbitMQService, RabbitMQService>();

        var serviceProvider = services.BuildServiceProvider();
        _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
    }

    [Test]
    public async Task CreateQueuesAsync_ShouldCreateQueues()
    {
        // Arrange
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqContainer.Hostname,
            UserName = rabbitMQOptions.UserName,
            Password = rabbitMQOptions.Password,
        };

        var optionsMock = new Mock<IOptions<RabbitMQOptions>>();
        optionsMock.Setup(o => o.Value).Returns(rabbitMQOptions);

        var service = new RabbitMQService(optionsMock.Object);

        // Act
        await service.CreateQueuesAsync();

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        // Assert
        Assert.IsTrue(channel.QueueDeclarePassive(RabbitMQQueues.ADD_OFFICE_QUEUE) != null);
        Assert.IsTrue(channel.QueueDeclarePassive(RabbitMQQueues.UPDATE_OFFICE_QUEUE) != null);
        Assert.IsTrue(channel.QueueDeclarePassive(RabbitMQQueues.DELETE_OFFICE_QUEUE) != null);
    }

    [Test]
    public async Task PublishMessageAsync_ShouldPublishMessage()
    {
        // Arrange
        var message = new { Id = 1, Name = "Test Office"};
        var queueName = RabbitMQQueues.ADD_OFFICE_QUEUE;

        // Act
        await _rabbitMQService.PublishMessageAsync(message, queueName);

        using var connection = new ConnectionFactory
        {
            HostName = _rabbitMqContainer.Hostname,
            UserName = rabbitMQOptions.UserName,
            Password = rabbitMQOptions.Password,
        }.CreateConnection();

        // Assert
        using var channel = connection.CreateModel();
        var result = channel.BasicGet(queueName, autoAck: true);

        Assert.IsNotNull(result, "A message should have been published to the queue.");
        var body = Encoding.UTF8.GetString(result.Body.ToArray());
        var receivedMessage = JsonConvert.DeserializeObject<dynamic>(body);

        Assert.AreEqual(message.Name, receivedMessage.Name.ToString());
        Assert.AreEqual(message.Id, (int)receivedMessage.Id);
    }
}
