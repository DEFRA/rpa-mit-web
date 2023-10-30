using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.Summary;
using EST.MIT.Web.Shared.Components.SummaryPaymentRequest;

namespace EST.MIT.Web.Tests.Components;

public class SummaryPaymentRequestTests : TestContext
{
    private readonly PaymentRequest _paymentRequest;
    public SummaryPaymentRequestTests()
    {
        _paymentRequest = new PaymentRequest()
        {
            FRN = 1234567890,
            SourceSystem = "",
            MarketingYear = 0,
            PaymentRequestNumber = 0,
            AgreementNumber = "",
            Value = 0,
            DueDate = "",
            Currency = "GBP",
            InvoiceLines = new List<InvoiceLine>()
        };

        _paymentRequest.InvoiceLines.Add(new InvoiceLine()
        {
            Value = 0,
            Description = "",
            SchemeCode = "",
            DeliveryBody = "RP00"
        });
    }

    [Fact]
    public void SummaryPaymentRequest_Parameters_Are_Set()
    {
        var component = RenderComponent<SummaryPaymentRequest>(parameters =>
        {
            parameters.Add(p => p.PaymentRequest, _paymentRequest);
            parameters.Add(p => p._Parent, new Summary() { invoiceId = "testid", scheme = "testscheme" });
        });


        component.Instance.PaymentRequest.Should().NotBeNull();
        component.Instance.PaymentRequest.FRN = _paymentRequest.FRN;
        component.Instance.PaymentRequest.SourceSystem = _paymentRequest.SourceSystem;
        component.Instance.PaymentRequest.MarketingYear = _paymentRequest.MarketingYear;
        component.Instance.PaymentRequest.PaymentRequestNumber = _paymentRequest.PaymentRequestNumber;
        component.Instance.PaymentRequest.AgreementNumber = _paymentRequest.AgreementNumber;
        component.Instance.PaymentRequest.Currency = _paymentRequest.Currency;
        component.Instance.PaymentRequest.Value = _paymentRequest.Value;
        component.Instance.PaymentRequest.DueDate = _paymentRequest.DueDate;

        component.Instance.PaymentRequest.InvoiceLines.Should().NotBeNull();
        component.Instance.PaymentRequest.InvoiceLines.Count.Should().Be(1);
        component.Instance.PaymentRequest.InvoiceLines[0].Value = _paymentRequest.InvoiceLines[0].Value;
        component.Instance.PaymentRequest.InvoiceLines[0].SchemeCode = _paymentRequest.InvoiceLines[0].SchemeCode;
        component.Instance.PaymentRequest.InvoiceLines[0].Description = _paymentRequest.InvoiceLines[0].Description;
        component.Instance.PaymentRequest.InvoiceLines[0].DeliveryBody = _paymentRequest.InvoiceLines[0].DeliveryBody;

    }
}