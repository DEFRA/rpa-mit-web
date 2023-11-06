using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.DeletePaymentRequestConfirmation;

public partial class DeletePaymentRequestConfirmation : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;

    private async Task ConfirmDelete()
    {
        var response = await _api.DeletePaymentRequestAsync(_invoiceStateContainer.Value, PaymentRequestId);

        if (response.IsSuccess)
        {
            if (response.Data != null)
            {
                _invoiceStateContainer.SetValue(response.Data);
                _nav.NavigateTo($"/invoice/summary/{_invoiceStateContainer.Value.SchemeType}/{_invoiceStateContainer.Value.Id}");
            }

            _nav.NavigateTo($"/invoice/summary/{_invoiceStateContainer.Value.SchemeType}/{_invoiceStateContainer.Value.Id}");
        }

    }

    private void CancelDelete()
    {
        _nav.NavigateTo($"/invoice/summary/{_invoiceStateContainer.Value.SchemeType}/{_invoiceStateContainer.Value.Id}");
    }
}