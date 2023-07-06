using System.ComponentModel.DataAnnotations;
namespace Entities;

public class AccountSelect
{
    [Required(ErrorMessage = "Please select an account type")]
    public string Account { get; set; } = default!;
}

public class OrganisationSelect
{
    [Required(ErrorMessage = "Please select an organisation")]
    public string Organisation { get; set; } = default!;
}

public class SchemeSelect
{
    [Required(ErrorMessage = "Please select a scheme type")]
    public string Scheme { get; set; } = default!;
}

public class PaymentTypeSelect
{
    [Required(ErrorMessage = "Please select a payment type")]
    public string PaymentType { get; set; } = default!;
}