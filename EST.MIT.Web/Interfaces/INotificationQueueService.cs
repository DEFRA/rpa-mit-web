namespace EST.MIT.Web.Interfaces;

public interface INotificationQueueService
{
    Task<bool> AddMessageToQueueAsync(string message, string data);
}
