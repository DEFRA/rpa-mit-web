using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IImporterQueueService
{
    Task<bool> AddMessageToQueueAsync(ImportRequest request);
}
