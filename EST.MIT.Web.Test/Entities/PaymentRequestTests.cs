using System.ComponentModel.DataAnnotations;
using FluentAssertions;

namespace Entities.Tests;

public class PaymentRequestTests : TestContext
{

    [Fact]
    public void PaymentRequestId_Is_Generated()
    {
        var paymentRequest = new PaymentRequest();
        paymentRequest.PaymentRequestId.Should().NotBeEmpty();
    }

    [Fact]
    public void FRN_Is_Required()
    {
        var paymentRequest = new PaymentRequest();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.FRN)));
    }

    [Fact]
    public void FRN_Should_Be_10_Digits()
    {
        var paymentRequest = new PaymentRequest()
        {
            FRN = 123456789
        };

        var validationResults = ValidateModel(paymentRequest);
        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The FRN must be 10 digits");

    }

    [Fact]
    public void MarketingYear_Is_After_2014()
    {
        var paymentRequest = new PaymentRequest();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Marketing Year must be after than 2014");
    }

    [Fact]
    public void PaymentRequestNumber_Is_Required()
    {
        var paymentRequest = new PaymentRequest();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.PaymentRequestNumber)));
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
    public void Value_Should_Be_Currency_Format()
    {
        var paymentRequest = new PaymentRequest()
        {
            Value = 12.345
        };

        var validationResults = ValidateModel(paymentRequest);
        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Value must be in the format 0.00");
    }

    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}