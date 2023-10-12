using Entities;
using EST.MIT.Web.Shared;
using Helpers;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.invoice.AmendInvoiceLine;

public partial class AmendInvoiceLine : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = "";
    [Parameter] public string InvoiceLineId { get; set; } = "";

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private InvoiceLine invoiceLine;
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice ??= _invoiceStateContainer.Value;
        paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);
        invoiceLine ??= paymentRequest?.InvoiceLines.First(x => x.InvoiceLineId.ToString() == InvoiceLineId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (paymentRequest.IsNull() || invoice.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }

    private async Task UpdateInvoiceLine()
    {
        if (!_pageServices.Validation(invoiceLine, out IsErrored, out errors)) return;

        paymentRequest.InvoiceLines = paymentRequest.InvoiceLines.Where(x => x.InvoiceLineId.ToString() != InvoiceLineId).ToList();
        paymentRequest.InvoiceLines.Add(invoiceLine);

        var response = await _api.UpdateInvoiceAsync(invoice);

        if (response.IsSuccess)
        {
            _invoiceStateContainer.SetValue(response.Data);
            _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
        }

        IsErrored = true;
        errors = response.Errors;
    }

    private void Cancel()
    {
        _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
    }
}