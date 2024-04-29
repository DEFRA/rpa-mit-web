using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.find.Find;

public partial class Find : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IFindService _findService { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private ILogger<Find> Logger { get; set; }

    private Dictionary<string, List<string>> errors = new();
    private bool IsErrored = false;
    private bool NotFound = false;
    public Invoice Invoice { get { return _invoice; } }

    private Invoice _invoice = default!;

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
        if (!_pageServices.Validation(_searchCriteria, out IsErrored, out errors)) return;

        errors = new();
        _invoice = await _findService.FindInvoiceAsync(_searchCriteria);

        if (Invoice.IsNull())
        {
            NotFound = true;
            IsErrored = false;
            return;
        }
        _nav.NavigateTo($"/invoice/summary/{Invoice.SchemeType}/{Invoice.Id}");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(_searchCriteria, out IsErrored, out errors);
    }
}