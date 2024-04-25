using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.create_bulk.Review;

public partial class Review : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private ILogger<Review> Logger { get; set; }

    private Invoice invoice { get; set; }
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            invoice = _invoiceStateContainer.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing Review page");
            _nav.NavigateTo("/error");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            invoice = _invoiceStateContainer.Value;

            if (invoice == null)
            {
                _nav.NavigateTo("/create-bulk");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error rendering Review page");
            _nav.NavigateTo("/error");
        }
    }


    private void ValidationFailed()
    {
        _pageServices.Validation(invoice, out IsErrored, out errors);
    }

    private void Continue()
    {
        _nav.NavigateTo($"/bulk/");
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}