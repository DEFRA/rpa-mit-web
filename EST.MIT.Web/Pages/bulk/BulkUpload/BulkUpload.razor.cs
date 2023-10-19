using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Services;
using Entities;

namespace EST.MIT.Web.Pages.bulk.BulkUpload;

public partial class BulkUpload : ComponentBase
{
    [Inject] private IUploadService _uploadService { get; set; }
    [Inject] private ILogger<BulkUpload> _logger { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    public BulkUploadFileSummary fileToLoadSummary = default!;
    public bool error = false;
    public string errorMessage = String.Empty;

    [Parameter] public string schemeType { get; set; }
    [Parameter] public string organisation { get; set; }
    [Parameter] public string paymentType { get; set; }
    [Parameter] public string accountType { get; set; }
    [Parameter] public string createdBy { get; set; }   

    protected override void OnInitialized()
    {
        fileToLoadSummary = new BulkUploadFileSummary();
    }

    private void FileLoaded(InputFileChangeEventArgs e)
    {
        fileToLoadSummary.Errors.Clear();
        error = false;
        errorMessage = String.Empty;
        fileToLoadSummary.File = e.File;

        if (!fileToLoadSummary.IsValid())
        {
            error = true;
            errorMessage = string.Join(", ", fileToLoadSummary.Errors);
            _logger.LogError(errorMessage);
        }
    }

    private async Task UploadFile()
    {
        if (fileToLoadSummary.IsValidFile)
        {
            fileToLoadSummary.UploadResponse = await _uploadService.UploadFileAsync(fileToLoadSummary.File,schemeType,organisation,paymentType,accountType,createdBy);
            fileToLoadSummary.IsUploaded = fileToLoadSummary.UploadResponse.IsSuccessStatusCode;

            if (!fileToLoadSummary.IsUploaded)
            {
                error = true;
                errorMessage = string.Join(",", await fileToLoadSummary.UploadResponse.Content.ReadAsStringAsync());
                _logger.LogError(errorMessage);
            }

            var confirmationNumber = await fileToLoadSummary.UploadResponse.Content.ReadAsStringAsync();
            _nav.NavigateTo($"/bulk/confirmation/{confirmationNumber}");

        }
    }
}
