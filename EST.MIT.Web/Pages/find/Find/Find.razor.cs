using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.find.Find;

public partial class Find : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IFindService _findService { get; set; }
    [Inject] private IPageServices _pageService { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }
    [Inject] private ILogger<Find> Logger { get; set; }

    private Dictionary<string, List<string>> errors = new();
    private bool IsErrored, NotFound = false;
    private Invoice invoice = default!;
    public SearchCriteria _searchCriteria = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing Find page");
            _nav.NavigateTo("/error");
        }
    }

    public async Task Search()
    {
        invoice = await _findService.FindInvoiceAsync(_searchCriteria);

        if (invoice.IsNull())
        {
            NotFound = true;
            IsErrored = false;
            errors = new();
            return;
        }
        _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
    }

    private void ValidationFailed()
    {
        _pageService.Validation(_searchCriteria, out IsErrored, out errors);
    }
}