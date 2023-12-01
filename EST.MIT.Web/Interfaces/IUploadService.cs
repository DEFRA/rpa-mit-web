using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace EST.MIT.Web.Interfaces;

public interface IUploadService
{
    Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, Invoice invoice);
}
