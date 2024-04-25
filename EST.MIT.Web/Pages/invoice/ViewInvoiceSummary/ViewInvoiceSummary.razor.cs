using System.Net;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Pages.invoice.ViewInvoiceSummary;

public partial class ViewInvoiceSummary : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string invoiceId { get; set; } = "";

    [Parameter] public string scheme { get; set; } = "";

    [Parameter] public string backUrl { get; set; } = "";

    private Invoice invoice;

    private bool readOnly;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = await _api.FindInvoiceAsync(invoiceId, scheme);
        _invoiceStateContainer.SetValue(invoice);
        readOnly = invoice?.Status != InvoiceStatuses.New;
    }

    private void AddPaymentRequest()
    {
        _nav.NavigateTo("/invoice/add-payment-request");
    }

    private void SendForApproval()
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