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

    // TODO: fxs replace with model validator, ctor will set default to 1
    //[Fact]
    //public void PaymentRequestNumber_Is_Required()
    //{
    //    var paymentRequest = new PaymentRequest();
    //    var validationResults = ValidateModel(paymentRequest);

    //    validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.PaymentRequestNumber)));
    //}

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

    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}