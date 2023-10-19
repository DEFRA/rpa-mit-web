using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Shared;
using Entities;
using EST.MIT.Web.Entities;
using Services;

namespace EST.MIT.Web.Pages.invoice.UpdatePaymentRequest;

public partial class UpdatePaymentRequest : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;

    private Invoice invoice => _invoiceStateContainer.Value;
    private PaymentRequest paymentRequest = default!;
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        paymentRequest = invoice.PaymentRequests.FirstOrDefault(x => x.PaymentRequestId == PaymentRequestId) ?? new PaymentRequest();
    }

    private async Task SavePaymentRequest()
    {
        if (!_pageServices.Validation(paymentRequest, out IsErrored, out errors)) return;

        invoice.PaymentRequests.Add(paymentRequest);
        var response = await _api.UpdateInvoiceAsync(invoice, paymentRequest);

        if (response.IsSuccess)
        {
            _invoiceStateContainer.SetValue(invoice);
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