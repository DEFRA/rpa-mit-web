using System.Diagnostics.CodeAnalysis;
using EST.MIT.Web.Entities;
using Newtonsoft.Json;

namespace EST.MIT.Web.Services;

public interface IPaymentService
{
    Task SendPayment(Invoice invoice);
}

/// <summary>
/// This is only intended to be a basic POC implementation
/// </summary>

[ExcludeFromCodeCoverage]
public class PaymentService : IPaymentService
{
    public readonly IServiceBusProvider _serviceBusProvider;

    public PaymentService(IServiceBusProvider serviceBusProvider)
    {
        _serviceBusProvider = serviceBusProvider;
    }

    public async Task SendPayment(Invoice invoice)
    {
        var payment = GeneratePayment(invoice);
        // TODO: Change the queue name to use the PaymentGeneratorQueueName config value
        await _serviceBusProvider.SendMessageAsync("rpa-mit-payment", payment);
    }

    private string GeneratePayment(Invoice invoice)
    {
        var PaymentRequests = new List<PaymentRequest>();

        foreach(var paymentRequest in invoice.PaymentRequests)
        {
            var InvoiceLines = new List<InvoiceLine>();
            foreach(var invoiceLine in paymentRequest.InvoiceLines)
            {
                InvoiceLines.Add(new InvoiceLine(){
                    value = invoiceLine.Value,
                    schemeCode = invoiceLine.SchemeCode,
                    description = invoiceLine.Description,
                    fundCode = invoiceLine.FundCode,
                    mainAccount = invoiceLine.MainAccount,
                    marketingYear = int.Parse(invoiceLine.MarketingYear),
                    deliveryBody = invoiceLine.DeliveryBody
                });
            }

            PaymentRequests.Add(new PaymentRequest() {
                paymentRequests = new List<PaymentDetail>() {
                    new PaymentDetail() {
                        paymentRequestId = paymentRequest.PaymentRequestId,
                        frn = long.Parse(paymentRequest.FRN),
                        sbi = paymentRequest.SBI != "" ? int.Parse(paymentRequest.SBI) : 0,
                        vendor = paymentRequest.Vendor,
                        sourceSystem = "Manual",
                        marketingYear = int.Parse(paymentRequest.MarketingYear),
                        currency = paymentRequest.Currency,
                        description = paymentRequest.Description,
                        originalInvoiceNumber = paymentRequest.OriginalInvoiceNumber,
                        originalSettlementDate = paymentRequest.OriginalSettlementDate,
                        recoveryDate = paymentRequest.RecoveryDate,
                        invoiceCorrectionReference = paymentRequest.InvoiceCorrectionReference,
                        paymentRequestNumber = paymentRequest.PaymentRequestNumber,
                        agreementNumber = paymentRequest.AgreementNumber,
                        value = paymentRequest.Value,
                        dueDate = paymentRequest.DueDate,
                        invoiceLines = InvoiceLines
                    }
                },
                id = paymentRequest.PaymentRequestId,
                accountType = invoice.AccountType,
                organisation = invoice.Organisation,
                schemeType = invoice.SchemeType,
                paymentType = invoice.PaymentType,
                status = invoice.Status,
                reference = "a test reference",
                created = invoice.Created,
                updated = invoice.Updated,
                createdBy = invoice.CreatedBy,
                updatedBy = invoice.UpdatedBy
            });
        }

        var payment = new PaymentRequestBatch(){
            schemeType = invoice.SchemeType,
            paymentRequestsBatches = PaymentRequests
        };

        return JsonConvert.SerializeObject(payment);

    }
}

[ExcludeFromCodeCoverage]
public class PaymentRequestBatch
{
    public string schemeType { get; set; }
    public List<PaymentRequest> paymentRequestsBatches { get; set; }
}

[ExcludeFromCodeCoverage]
public class PaymentRequest
{
    public string id { get; set; }
    public string accountType { get; set; }
    public string organisation { get; set; }
    public string schemeType { get; set; }
    public string paymentType { get; set; }
    public List<PaymentDetail> paymentRequests { get; set; }
    public string status { get; set; }
    public string reference { get; set; } = default!;
    public DateTimeOffset created { get; set; }
    public DateTimeOffset updated { get; set; }
    public string createdBy { get; set; }
    public string updatedBy { get; set; }
}

[ExcludeFromCodeCoverage]
public class PaymentDetail
{
    public string paymentRequestId { get; set; }
    public long frn { get; set; } = default!;
    public int sbi { get; set; } = default!;
    public string vendor { get; set; }
    public string sourceSystem { get; set; }
    public int marketingYear { get; set; }
    public string currency { get; set; }
    public string description { get; set; }
    public string originalInvoiceNumber { get; set; }
    public DateTime originalSettlementDate { get; set; }
    public DateTime recoveryDate { get; set; }
    public string invoiceCorrectionReference { get; set; }
    public int paymentRequestNumber { get; set; }
    public string agreementNumber { get; set; }
    public decimal value { get; set; }
    public string dueDate { get; set; }
    public List<InvoiceLine> invoiceLines { get; set; }
}

[ExcludeFromCodeCoverage]
public class InvoiceLine
{
    public decimal value { get; set; }
    public string schemeCode { get; set; }
    public string description { get; set; }
    public string fundCode { get; set; }
    public string mainAccount { get; set; }
    public int marketingYear { get; set; }
    public string deliveryBody { get; set; }
}