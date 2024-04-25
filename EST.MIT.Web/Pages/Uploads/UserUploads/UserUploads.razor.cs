using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.Uploads.UserUploads;
public partial class UserUploads : ComponentBase
{

    [Inject] private IUploadAPI _API { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private ILogger<UserUploads> Logger { get; set; }


    private IEnumerable<ImportRequest> importRequests = new List<ImportRequest>();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            importRequests = await _API.GetUploadsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing UserUploads page");
            _nav.NavigateTo("/error");
        }
    }
}