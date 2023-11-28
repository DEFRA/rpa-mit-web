using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface INotificationQueueService
{
    Task<bool> AddMessageToQueueAsync(Notification request);
}
