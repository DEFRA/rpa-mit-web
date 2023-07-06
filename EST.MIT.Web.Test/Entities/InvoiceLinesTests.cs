using System.ComponentModel.DataAnnotations;
using FluentAssertions;

namespace Entities.Tests;

public class InvoiceLineTests : TestContext
{
    [Fact]
    public void Value_Is_Required()
    {
        var paymentRequest = new InvoiceLine();
        var validationResults = ValidateModel(paymentRequest);

        validationResults.Should().ContainSingle(vr => vr.MemberNames.Contains(nameof(PaymentRequest.Value)));
    }

    [Fact]
    public void Value_Should_Be_Currency_Format()
    {
        var paymentRequest = new InvoiceLine()
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