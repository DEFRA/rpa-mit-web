using Entities;
using EST.MIT.Web.Shared;
using Helpers;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.invoice.AddInvoiceLine;

public partial class AddInvoiceLine : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = "";

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private readonly InvoiceLine invoiceLine = new();
    private bool IsErrored = false;
    private Dictionary<string, string> errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
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

    private async Task SaveInvoiceLine()
    {
        if (!_pageServices.Validation(invoiceLine, out IsErrored, out errors)) return;

        paymentRequest.Value += invoiceLine.Value;
        var response = await _api.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine);

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