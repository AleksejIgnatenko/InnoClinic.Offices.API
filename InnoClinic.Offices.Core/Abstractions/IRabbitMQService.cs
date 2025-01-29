
namespace InnoClinic.Offices.Application.Services
{
    public interface IRabbitMQService
    {
        Task PublishMessageAsync(object obj, string queueName);
    }
}