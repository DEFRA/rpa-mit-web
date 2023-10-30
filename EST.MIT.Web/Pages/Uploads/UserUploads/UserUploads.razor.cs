using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Pages.Uploads.UserUploads;
public partial class UserUploads : ComponentBase
{

    [Inject] private IUploadAPI _API { get; set; }

    private IEnumerable<ImportRequest> importRequests = new List<ImportRequest>();


    protected override async Task OnInitializedAsync()
    {
        importRequests = await _API.GetUploadsAsync();
    }
}