using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IUploadAPI
{
    Task<IEnumerable<ImportRequest>> GetUploadsAsync();
    //Task<byte[]> GetFileByFileNameAsync(string fileName);
    Task<byte[]> GetFileByImportRequestIdAsync(Guid importRequestId);
}
