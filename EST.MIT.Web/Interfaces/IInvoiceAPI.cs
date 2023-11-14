using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IInvoiceAPI
{
    Task<Invoice> FindInvoiceAsync(string id, string scheme);
    Task<ApiResponse<Invoice>> SaveInvoiceAsync(Invoice invoice);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest, InvoiceLine invoiceLine);
    Task<ApiResponse<Invoice>> DeletePaymentRequestAsync(Invoice? invoice, string paymentRequestId);
    Task<IEnumerable<Invoice>> GetApprovalsAsync();
    Task<IEnumerable<Invoice>> GetInvoicesAsync();
}


