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
        var userName = "andy.gibbons@defra.gov.uk";

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

    [Fact]
    public void When_Invoice_Has_PaymentRequests_With_InvoiceLines_Values_Then_TotalValues_Is_Displayed()
    {
        //Arrange
        var invoice = new Invoice()
        {
            Id = new Guid(),
            AccountType = "AR",
            Organisation = "NE",
            SchemeType = "CS",
            PaymentType = "GBP",
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "9999999987",
                    MarketingYear = "2023",
                    Currency = "GBP",
                    SBI = "1",
                    AgreementNumber = "EXT345",
                    Value = 30.67M,
                    InvoiceLines = new List<InvoiceLine>()
                    {
                            new InvoiceLine()
                            {
                                Value = 30.67M
                            }
                    }
                },
                new PaymentRequest()
                {
                    FRN = "4599999987",
                    MarketingYear = "2023",
                    Currency = "GBP",
                    SBI = "1",
                    AgreementNumber = "DE34",
                    Value = 305.34M,
                    InvoiceLines = new List<InvoiceLine>()
                    {
                            new InvoiceLine()
                            {
                                Value = 305.34M
                            }
                    }
                },
                new PaymentRequest()
                {
                    FRN = "9999999987",
                    MarketingYear = "2023",
                    Currency = "EUR",
                    SBI = "1",
                    AgreementNumber = "CC4",
                    Value = 555.67M,
                    InvoiceLines = new List<InvoiceLine>()
                    {
                            new InvoiceLine()
                            {
                                Value = 555.67M
                            }
                    }
                },
                new PaymentRequest()
                {
                    FRN = "9999999911",
                    MarketingYear = "2023",
                    Currency = "EUR",
                    SBI = "4",
                    AgreementNumber = "CD4",
                    Value = 2.00M,
                    InvoiceLines = new List<InvoiceLine>()
                    {
                            new InvoiceLine()
                            {
                                Value = 2.00M
                            }
                    }
                }
            }
        };

        //Assert
        Assert.Equal(557.67M, invoice.TotalValueOfPaymentsEUR);
        Assert.Equal(336.01M, invoice.TotalValueOfPaymentsGBP);
        Assert.Equal(4, invoice.NumberOfPaymentRequests);
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With_No_Invoicelines_Then_Invoice_Cannot_Be_Approved()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 0,
                    DueDate = "",
                    Currency = "GBP"
                }
            }
        };

        //Assert
        Assert.False(invoice.CanBeSentForApproval);
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With_At_Least_One_InvoiceLine_With_Value_NotEqualTo_Zero_Then_Invoice_Can_Be_Approved()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 34.89M,
                    DueDate = "",
                    Currency = "GBP",
                    InvoiceLines = new List<InvoiceLine>()
                    {
                        new InvoiceLine()
                        {
                             Value = 34.89M
                        }
                    }
                }
            }
        };

        //Assert 
        Assert.True(invoice.CanBeSentForApproval);
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With_No_InvoiceLines_Then_Invoice_Cannot_Be_Approved()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 0,
                    DueDate = "",
                    Currency = "GBP",
                    InvoiceLines = new List<InvoiceLine>()
                    {
                        new InvoiceLine()
                        {
                             Value = 34.89M
                        },
                        new InvoiceLine()
                        {
                            Value= 23.90M
                        }
                    }
                },

                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 0,
                    DueDate = "",
                    Currency = "GBP"
                }
            }
        };

        //Assert 
        Assert.False(invoice.CanBeSentForApproval);
    }

    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
