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
    public void JsonConstructor_Should_Initialize_Properties_Correctly()
    {
        var id = Guid.NewGuid();
        var paymentType = "Credit Card";
        var accountType = "Personal";
        var organisation = "OrgName";
        var schemeType = "TypeA";
        var paymentRequests = new List<PaymentRequest>();
        var status = "Pending";
        var reference = "Ref123";
        var created = DateTimeOffset.Now;
        var updated = DateTimeOffset.Now;
        var approverId = "ApproverId";
        var approverEmail = "email@example.com";
        var approvedBy = "ApprovedBy";
        DateTime approved = DateTime.Now;
        var createdBy = "CreatedBy";
        var updatedBy = "UpdatedBy";

        var invoice = new Invoice(id, paymentType, accountType, organisation, schemeType, paymentRequests, status, reference, created, updated, createdBy, updatedBy, approverId, approverEmail, approvedBy, approved);

        invoice.Id.Should().Be(id);
        invoice.PaymentType.Should().Be(paymentType);
        invoice.AccountType.Should().Be(accountType);
        invoice.Organisation.Should().Be(organisation);
        invoice.SchemeType.Should().Be(schemeType);
        invoice.PaymentRequests.Should().BeEquivalentTo(paymentRequests);
        invoice.Status.Should().Be(status);
        invoice.Reference.Should().Be(reference);
        invoice.Created.Should().Be(created);
        invoice.Updated.Should().Be(updated);
        invoice.ApproverId.Should().Be(approverId);
        invoice.ApproverEmail.Should().Be(approverEmail);
        invoice.ApprovedBy.Should().Be(approvedBy);
        invoice.Approved.Should().Be(approved);
        invoice.CreatedBy.Should().Be(createdBy);
        invoice.UpdatedBy.Should().Be(updatedBy);
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

    [Fact]
    public void Update_Should_Update_Fields_Correctly()
    {
        var invoice = new Invoice();
        var newStatus = "updated";

        invoice.Update(newStatus);

        invoice.Updated.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMinutes(1));
        invoice.UpdatedBy.Should().Be("user");
        invoice.Status.Should().Be(newStatus);
    }

    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
