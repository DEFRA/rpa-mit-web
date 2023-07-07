using Microsoft.AspNetCore.Components;
using Services;
using Helpers;
using Entities;

namespace EST.MIT.Web.Pages.find.Find;

public partial class Find : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IFindService _findService { get; set; }
    [Inject] private IPageServices _pageService { get; set; }

    private Dictionary<string, List<string>> errors = new();
    private bool IsErrored, NotFound = false;
    public Invoice invoice = default!;
    public SearchCriteria _searchCriteria = new();

    public async Task Search()
    {
        invoice = await _findService.FetchInvoiceAsync(_searchCriteria.InvoiceNumber, _searchCriteria.Scheme);

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