using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Entities;


namespace Entities.Tests;

public class PaymentRequestTests : TestContext
{

    [Fact]
    public void PaymentRequestId_Is_Generated()
    {
        var paymentRequest = new PaymentRequest();
        paymentRequest.PaymentRequestId.Should().NotBeEmpty();
    }

    //TODO: fxs replace with model validator
    //[Fact]
    //public void FRN_Is_Required()
    //{
    //    var paymentRequest = new PaymentRequest();
    //    var validationResults = ValidateModel(paymentRequest);

    //    validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.FRN)));
    //}

    [Fact]
    public void FRN_Should_Be_10_Digits()
    {
        var paymentRequest = new PaymentRequest()
        {
            FRN = "123456789"
        };

        var validationResults = ValidateModel(paymentRequest);
        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The FRN must be a 10-digit number or be empty.");

    }

    [Fact]
    public void SBI_Should_Be_GreaterThanMinRange()
    {
        var paymentRequest = new PaymentRequest()
        {
            SBI = "104999999"
        };

        var validationResults = ValidateModel(paymentRequest);
        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The SBI is not in valid range (105000000 .. 999999999) or should be empty.");

    }

    [Fact]
    public void SBI_Should_Be_LessThanMaxRange()
    {
        var paymentRequest = new PaymentRequest()
        {
            SBI = "1000000000"
        };

        var validationResults = ValidateModel(paymentRequest);
        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The SBI is not in valid range (105000000 .. 999999999) or should be empty.");

    }

    [Fact]
    public void MarketingYear_Is_After_2014()
    {
        var paymentRequest = new PaymentRequest();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Marketing Year must be after 2014");
    }

    [Fact]
    public void PaymentRequestNumber_Is_SetTo1_InConstructor()
    {
        var paymentRequest = new PaymentRequest();

        Assert.Equal(1, paymentRequest.PaymentRequestNumber);
    }


    [Fact]
    public void AgreementNumber_Is_Required()
    {
        var paymentRequest = new PaymentRequest();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.AgreementNumber)));
    }

    [Fact]
    public void Currency_Is_Required()
    {
        var paymentRequest = new PaymentRequest();
        paymentRequest.Currency = string.Empty;
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.Currency)));
    }

    [Fact]
    public void Currency_Must_Be_GBPorEUR()
    {
        var paymentRequest = new PaymentRequest();
        paymentRequest.Currency = "USD";
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.Currency)));
    }

    [Fact]
    public void AR_Specific_Fields_Are_Required()
    {
        var ARpaymentRequest = new PaymentRequest() { AccountType = "AR" };
        var ARvalidationResults = ValidateModel(ARpaymentRequest);
        ARvalidationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.OriginalInvoiceNumber)));
        ARvalidationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.OriginalSettlementDate)));
        ARvalidationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.RecoveryDate)));
        ARvalidationResults.Should().NotContain(vr => vr.MemberNames.Contains(nameof(PaymentRequest.InvoiceCorrectionReference)));

        var APpaymentRequest = new PaymentRequest() { AccountType = "AP" };
        var APvalidationResults = ValidateModel(APpaymentRequest);
        APvalidationResults.Should().NotContain(vr => vr.MemberNames.Contains(nameof(PaymentRequest.OriginalInvoiceNumber)));
        APvalidationResults.Should().NotContain(vr => vr.MemberNames.Contains(nameof(PaymentRequest.OriginalSettlementDate)));
        APvalidationResults.Should().NotContain(vr => vr.MemberNames.Contains(nameof(PaymentRequest.RecoveryDate)));
        APvalidationResults.Should().NotContain(vr => vr.MemberNames.Contains(nameof(PaymentRequest.InvoiceCorrectionReference)));
    }

    [Fact]
    public void Validate_ShouldGenerateError_WhenFRNSBIAndVendorAreEmpty()
    {
        var paymentRequest = new PaymentRequest
        {
        };

        var validationContext = new ValidationContext(paymentRequest, serviceProvider: null, items: null);

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(paymentRequest, validationContext, results, true);

        var validateResults = paymentRequest.Validate(validationContext);

        Assert.False(isValid, "Validation should fail when FRN, SBI, and Vendor are empty.");

        Assert.Contains(validateResults, v => v.ErrorMessage == "At least one of FRN, SBI, or Vendor must be entered");
    }



    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}