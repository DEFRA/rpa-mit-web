using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Helpers;

public static class ValidatorHelper
{
    public static IEnumerable<ValidationResult> Validate(this object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);

        if (model is IValidatableObject validatableObject)
        {
            foreach (var validationResult in validatableObject.Validate(validationContext))
            {
                validationResults.Add(validationResult);
            }
        }

        return validationResults;
    }
}

