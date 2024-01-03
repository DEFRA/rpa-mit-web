using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.ViewInvoiceList;
public partial class ViewInvoiceList : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceAPI _API { get; set; }

    private IEnumerable<Invoice> invoices = new List<Invoice>();


    protected override async Task OnInitializedAsync()
    {
        try
        {
            invoices = await _API.GetInvoicesAsync();
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error retrieving invoices: {ex.Message}");
            _nav.NavigateTo("/error");
        }
    }
}