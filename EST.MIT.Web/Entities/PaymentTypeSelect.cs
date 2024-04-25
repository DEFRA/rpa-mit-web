using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class PaymentTypeSelect
{
    [Required(ErrorMessage = "Please select a payment type")]
    public string PaymentType { get; set; } = default!;
}