using System.ComponentModel.DataAnnotations;
using Entities;

namespace Services.Tests;

public class PageServicesTests
{

    [Fact]
    public void Validation_ReturnsNoValidationResults_And_IsErroredIsFalse()
    {
        var pageServices = new PageServices();
        var invoice = new Invoice()
        {
            AccountType = "AR",
            Organisation = "RPA",
            SchemeType = "BPS",
            PaymentType = "First"
        };

        var result = pageServices.Validation(invoice, out bool IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeTrue();
        IsErrored.Should().BeFalse();
        errors.Should().NotBeNull();
        errors.Should().BeEmpty();
    }


    [Fact]
    public void Validation_Invoice_With_No_PaymentRequests_Returns_False()
    {
        var invoice = new Invoice();
        var pageServices = new PageServices();

        var result = pageServices.Validation(invoice, out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeFalse();
        IsErrored.Should().BeTrue();
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validation_Invoice_With_PaymentRequests_Returns_True()
    {
        var invoice = new Invoice();
        invoice.PaymentRequests.Add(new PaymentRequest()
        {
            CustomerId = "1234567890",
            ClaimReferenceNumber = "C123",
            Value = 123.45,
            Currency = "GBP"
        });

        var pageServices = new PageServices();

        var result = pageServices.Validation(invoice.PaymentRequests[0], out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeTrue();
        IsErrored.Should().BeFalse();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validation_PaymentRequest_With_No_InvoiceLines_Returns_False()
    {

        var paymentRequest = new PaymentRequest()
        {
            CustomerId = "1234567890",
            Value = 0,
            InvoiceLines = new List<InvoiceLine>(),
            Currency = "GBP"
        };

        var pageServices = new PageServices();

        var result = pageServices.Validation(paymentRequest, out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeFalse();
        IsErrored.Should().BeTrue();
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validation_PaymentRequest_With_InvoiceLines_Returns_True()
    {
        var paymentRequest = new PaymentRequest()
        {
            CustomerId = "1234567890",
            ClaimReferenceNumber = "C123",
            Value = 123.45,
            InvoiceLines = new List<InvoiceLine>(),
            Currency = "GBP"
        };

        paymentRequest.InvoiceLines.Add(new InvoiceLine()
        {
            Value = 123.45,
            Description = "Payment",
            SchemeCode = "Test",
            DeliveryBody = "RP00",
            MarketingYear = 2023
        });

        var pageServices = new PageServices();

        var result = pageServices.Validation(paymentRequest.InvoiceLines[0], out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeTrue();
        IsErrored.Should().BeFalse();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Null_Model_Returns_False()
    {
        Invoice invoice = default!;
        var pageServices = new PageServices();

        var result = pageServices.Validation(invoice, out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeFalse();
        IsErrored.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_Multiple_Errors_By_Key()
    {
        var testClass = new ErrorTestClass()
        {
            Test = "test"
        };

        var pageServices = new PageServices();

        var result = pageServices.Validation(testClass, out var IsErrored, out Dictionary<string, List<string>> errors);

        result.Should().BeFalse();
        IsErrored.Should().BeTrue();
        errors.Should().NotBeEmpty();
        errors.Should().HaveCount(1);
        errors.Should().ContainKey("test");
        errors["test"].Should().HaveCount(2);

    }

    public class ErrorTestClass
    {
        [Required]
        [EmailFormatCheck]
        [DomainCheck]
        public string Test { get; set; } = default!;
    }

}