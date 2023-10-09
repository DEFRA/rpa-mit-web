using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Shared;
using Entities;
using Helpers;
using Services;

namespace EST.MIT.Web.Pages.invoice.AmendPaymentRequest;

public partial class AmendPaymentRequest : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        invoice ??= _invoiceStateContainer.Value;
        paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);
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

    private void UpdatePaymentRequest()
    {
        _nav.NavigateTo($"/invoice/update-payment-request/{PaymentRequestId}");
    }

    private void AddInvoiceLine()
    {
        _nav.NavigateTo($"/invoice/add-invoice-line/{PaymentRequestId}");
    }

    private async Task DeleteInvoiceLine(int index)
    {
        paymentRequest.InvoiceLines.RemoveAt(index);

        var response = await _api.UpdateInvoiceAsync(invoice);

        if (response.IsSuccess)
        {
            _invoiceStateContainer.SetValue(response.Data);
            _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
        }

        _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
    }
}