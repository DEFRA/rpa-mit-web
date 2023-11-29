using System.Net;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.Summary;

public partial class Summary : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string invoiceId { get; set; } = "";

    [Parameter] public string scheme { get; set; } = "";

    [Parameter] public string backUrl { get; set; } = "";

    private Invoice invoice;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = await _api.FindInvoiceAsync(invoiceId, scheme);
        _invoiceStateContainer.SetValue(invoice);

    }

    private async Task AddPaymentRequest()
    {
        _nav.NavigateTo("/invoice/add-payment-request");
    }

    private async Task SendForApproval()
    {
        _nav.NavigateTo("/approval/select");
    }

    private string GetBackUrl()
    {
        if (!string.IsNullOrWhiteSpace(this.backUrl))
        {
            return WebUtility.UrlDecode(this.backUrl);
        }
        return "/";
    }
}