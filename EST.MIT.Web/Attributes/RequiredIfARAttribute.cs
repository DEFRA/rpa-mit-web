using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Attributes;

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

        if (accountTypeValue != null && accountTypeValue.ToString() == "AR" &&
            (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.Equals(Activator.CreateInstance(value.GetType()))))
        {
            string displayName = validationContext.DisplayName ?? validationContext.MemberName;
            if (string.IsNullOrEmpty(displayName) || displayName == validationContext.MemberName)
            {
                var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                var displayAttribute = property?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;

                displayName = displayAttribute?.DisplayName ?? validationContext.MemberName;
            }
            return new ValidationResult($"The {displayName} field is required when AccountType is AR.", new[] { validationContext.MemberName! });
        }


        return ValidationResult.Success;
    }
}