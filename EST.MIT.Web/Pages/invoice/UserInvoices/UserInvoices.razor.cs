using Entities;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.invoice.UserInvoices;
public partial class UserInvoices : ComponentBase
{
    [Inject] private IInvoiceAPI _API { get; set; }

    private IEnumerable<Invoice> invoices = new List<Invoice>();


    protected override async Task OnInitializedAsync()
    {
        invoices = await _API.GetApprovalsAsync();
    }
}