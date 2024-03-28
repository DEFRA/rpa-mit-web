using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace EST.MIT.Web.Services;

public interface IServiceBusProvider
{
    Task SendMessageAsync(string queue, string msg);
}


public class ServiceBusProvider : IServiceBusProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString = default!;

    public ServiceBusProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration["QueueConnectionString"];
    }

    public async Task SendMessageAsync(string queue, string msg)
    {
        await using var client = new ServiceBusClient(_connectionString);
        ServiceBusSender sender = client.CreateSender(queue);
        ServiceBusMessage message = new ServiceBusMessage(msg.EncodeMessage());
        await sender.SendMessageAsync(message);
    }

}