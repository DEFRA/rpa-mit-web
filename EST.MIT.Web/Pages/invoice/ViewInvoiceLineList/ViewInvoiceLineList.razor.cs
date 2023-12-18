using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.ViewInvoiceLineList;

public partial class ViewInvoiceLineList : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private readonly string backUrl = "user-invoices";
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

    private void UpdateInvoiceLine(Guid invoiceLineId)
    {
        _nav.NavigateTo($"/invoice/edit-invoice-line/{PaymentRequestId}/{invoiceLineId}");
    }

    private async Task DeleteInvoiceLine(Guid invoiceLineId)
    {
        paymentRequest.InvoiceLines = paymentRequest.InvoiceLines.Where(x => x.Id != invoiceLineId).ToList();

        var response = await _api.UpdateInvoiceAsync(invoice);

        if (response.IsSuccess)
        {
            _invoiceStateContainer.SetValue(response.Data);
        }

        _nav.NavigateTo($"/invoice/edit-payment-request/{PaymentRequestId}");
    }
}