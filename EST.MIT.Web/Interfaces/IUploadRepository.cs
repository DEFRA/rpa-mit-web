namespace EST.MIT.Web.Interfaces;

public interface IUploadRepository
{
    Task<HttpResponseMessage> GetUploads();
    //Task<HttpResponseMessage> GetFileByFileNameAsync(string fileName);
    Task<HttpResponseMessage> GetFileByImportRequestIdAsync(Guid importRequestId);
}
