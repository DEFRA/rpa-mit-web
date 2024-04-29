using Microsoft.AspNetCore.Components.Forms;

namespace EST.MIT.Web.Interfaces;

public interface IUploadService
{
    Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, string schemeType, string organisation, string paymentType, string accountType, string createdBy, string userEmail);
}
