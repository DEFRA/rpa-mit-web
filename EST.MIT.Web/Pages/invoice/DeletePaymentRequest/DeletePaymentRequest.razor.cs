using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.DeletePaymentRequest;

public partial class DeletePaymentRequest : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;
    private readonly string backUrl = "user-invoices";

    private async Task ConfirmDelete()
    {
        var invoice = _invoiceStateContainer.Value;
        if (invoice is not null)
        {
            var response = await _api.DeletePaymentRequestAsync(invoice, PaymentRequestId);

            if (response.IsSuccess)
            {
                if (response.Data != null)
                {
                    _invoiceStateContainer.SetValue(response.Data);
                    _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
                }

                _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
            }
        }
    }

    private void CancelDelete()
    {
        var invoice = _invoiceStateContainer.Value;
        if (invoice is not null)
        {
            _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}/{backUrl}");
        }
    }
}