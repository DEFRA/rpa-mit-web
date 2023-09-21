using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RequiredIfARAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var accountType = validationContext.ObjectType.GetProperty("AccountType");
        if (accountType == null)
        {
            return new ValidationResult("Property 'AccountType' not found.");
        }

        var accountTypeValue = accountType.GetValue(validationContext.ObjectInstance, null);

        if (accountTypeValue != null && accountTypeValue.ToString() == "AR" && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
        {
            return new ValidationResult($"The {validationContext.MemberName} field is required when AccountType is AR.", new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }
}
