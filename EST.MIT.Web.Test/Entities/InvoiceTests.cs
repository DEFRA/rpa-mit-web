using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Tests.Entities;

public class InvoiceTests
{
    [Fact]
    public void Id_Should_Be_Set_To_NewGuid_By_Default()
    {
        var invoice = new Invoice();

        invoice.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void PaymentType_Is_Required()
    {
        var invoice = new Invoice();
        var validationResults = ValidateModel(invoice);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "Payment Type is required");
    }

    [Fact]
    public void AccountType_Is_Required()
    {
        var invoice = new Invoice();
        var validationResults = ValidateModel(invoice);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "Account Type is required");
    }

    [Fact]
    public void Organisation_Is_Required()
    {
        var invoice = new Invoice();
        var validationResults = ValidateModel(invoice);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "Organisation is required");
    }

    [Fact]
    public void SchemeType_Is_Required()
    {
        var invoice = new Invoice();
        var validationResults = ValidateModel(invoice);

        validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "Scheme Type is required");
    }

    [Fact]
    public void Created_Should_Be_Set_To_Now_By_Default()
    {
        var invoice = new Invoice();

        invoice.Created.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Properties_Should_Be_Set_Correctly()
    {
        var id = new Guid("12345678-1234-1234-1234-123456789012");
        var paymentType = "Test Payment Type";
        var accountType = "Test Account Type";
        var organisation = "Test Organisation";
        var schemeType = "Test Scheme Type";
        var invoice = new Invoice
        {
            Id = id,
            PaymentType = paymentType,
            AccountType = accountType,
            Organisation = organisation,
            SchemeType = schemeType
        };

        Assert.Equal(id, invoice.Id);
        Assert.Equal(paymentType, invoice.PaymentType);
        Assert.Equal(accountType, invoice.AccountType);
        Assert.Equal(organisation, invoice.Organisation);
        Assert.Equal(schemeType, invoice.SchemeType);
    }

    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
