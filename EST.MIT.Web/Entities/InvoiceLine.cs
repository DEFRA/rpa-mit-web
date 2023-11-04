using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class InvoiceLine : Validatable
{
    public Guid Id { get; set; }

    //[Required]
    //[RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    //public double Value { get; set; }

    [Required]
<<<<<<< Updated upstream
    [RegularExpression(@"^[0-9]*(\.[0-9]{1,2})?$", ErrorMessage = "The value must be valid number and have a maximum of 2 decimal places.")]
    public double Value { get; set; }

=======
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public decimal Value { get; set; }
>>>>>>> Stashed changes
    [Required]
    public string Description { get; set; } = default!;

    [Required]
    public string FundCode { get; set; } = "";

    [Required]
    public string MainAccount { get; set; } = "";

    [Required]
    public string SchemeCode { get; set; } = "";

    [Required]
    public string MarketingYear { get; set; } = "";

    [Required]
    public string DeliveryBody { get; set; } = "";
}
