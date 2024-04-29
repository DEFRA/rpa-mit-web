using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;

namespace EST.MIT.Web.Pages.bulk.BulkUpload;

public partial class BulkUpload : ComponentBase
{
    [Inject] private IUploadService _uploadService { get; set; }
    [Inject] private ILogger<BulkUpload> _logger { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    [CascadingParameter]
    public MainLayout Layout { get; set; }

    public BulkUploadFileSummary fileToLoadSummary = default!;
    public bool error = false;
    public string errorMessage = String.Empty;
    private Invoice invoice { get; set; }

    protected override void OnInitialized()
    {
        fileToLoadSummary = new BulkUploadFileSummary();
        invoice = _invoiceStateContainer.Value;
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
        try
        {
            if (fileToLoadSummary.IsValidFile)
            {
                fileToLoadSummary.UploadResponse = await _uploadService.UploadFileAsync(fileToLoadSummary.File, invoice.SchemeType, invoice.Organisation, invoice.PaymentType, invoice.AccountType, invoice.CreatedBy, Layout.UserEmail);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during file upload");
            _nav.NavigateTo("/error");
        }
    }
}
