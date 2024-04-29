using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Entities;


namespace Entities.Tests;

public class SearchCriteriaTests : TestContext
{
    [Fact]
    public void Validate_ShouldNotGenerateError_WhenOneSearchCriteriaEntered()
    {
        var searchCriteria = new SearchCriteria
        {
            InvoiceNumber = Guid.NewGuid().ToString()
        };

        var validationContext = new ValidationContext(searchCriteria, serviceProvider: null, items: null);

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(searchCriteria, validationContext, results, true);

        Assert.True(isValid, "Validation should pass when one of Invoice Number and Payment Request Id are entered");

        searchCriteria.InvoiceNumber = "";
        searchCriteria.PaymentRequestId = "abcd_12345";

        isValid = Validator.TryValidateObject(searchCriteria, validationContext, results, true);

        Assert.True(isValid, "Validation should pass when one of Invoice Number and Payment Request Id are entered");
    }

    [Fact]
    public void Validate_ShouldGenerateError_WhenNoSearchCriteriaEntered()
    {
        var searchCriteria = new SearchCriteria
        {
        };

        var validationContext = new ValidationContext(searchCriteria, serviceProvider: null, items: null);

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(searchCriteria, validationContext, results, true);

        var validateResults = searchCriteria.Validate(validationContext);

        Assert.False(isValid, "Validation should fail when Invoice Number and Payment Request Id are empty.");

        Assert.Contains(validateResults, v => v.ErrorMessage == "An Invoice ID or Payment Requiest ID is required");
    }

    [Fact]
    public void Validate_ShouldGenerateError_WhenBothSearchCriteriaEntered()
    {
        var searchCriteria = new SearchCriteria
        {
            InvoiceNumber = Guid.NewGuid().ToString(),
            PaymentRequestId = "abcd_12345"
        };

        var validationContext = new ValidationContext(searchCriteria, serviceProvider: null, items: null);

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(searchCriteria, validationContext, results, true);

        var validateResults = searchCriteria.Validate(validationContext);

        Assert.False(isValid, "Validation should fail when Invoice Number and Payment Request Id are entered.");

        Assert.Contains(validateResults, v => v.ErrorMessage == "Only one of Invoice ID or Payment Requiest ID can be entered");
    }


    private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}