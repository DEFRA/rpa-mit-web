using System.ComponentModel.DataAnnotations;

namespace Helpers;

public static class ValidatorHelper
{
    public static IEnumerable<ValidationResult> Validate(this object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}

