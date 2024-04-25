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
    [Inject] private ILogger<ViewInvoiceLineList> Logger { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = default!;

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private readonly string backUrl = "user-invoices";
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override void OnInitialized()
    {
        try
        {
            base.OnInitialized();
            invoice ??= _invoiceStateContainer.Value;
            paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing EditPaymentRequest page");
            _nav.NavigateTo("/error");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (paymentRequest.IsNull() || invoice.IsNull())
            {
                _invoiceStateContainer.SetValue(null);
                _nav.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error rendering EditPaymentRequest page");
            _nav.NavigateTo("/error");
        }
    }

    private void UpdatePaymentRequest()
    {
        _nav.NavigateTo($"/invoice/edit-payment-request/{PaymentRequestId}");
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
        try
        {
            paymentRequest.InvoiceLines = paymentRequest.InvoiceLines.Where(x => x.Id != invoiceLineId).ToList();

            var response = await _api.UpdateInvoiceAsync(invoice);

            if (response.IsSuccess)
            {
                _invoiceStateContainer.SetValue(response.Data);
                _nav.NavigateTo($"/invoice/view-invoice-lines/{PaymentRequestId}");
            }
            else
            {
                IsErrored = true;
                errors = invoice.AllErrors;
                var invoiceBeforeEdit = await _api.FindInvoiceAsync(invoice.Id.ToString(), invoice.SchemeType);
                _invoiceStateContainer.SetValue(invoiceBeforeEdit);
                invoice = _invoiceStateContainer.Value;
                paymentRequest = invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);
            }

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing EditPaymentRequest page");
            _nav.NavigateTo("/error");
        }
    }
}
