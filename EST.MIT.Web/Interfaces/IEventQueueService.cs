namespace EST.MIT.Web.Interfaces;

public interface IEventQueueService
{
    Task<bool> AddMessageToQueueAsync(string message, string data);
}
