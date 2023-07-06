using System.ComponentModel.DataAnnotations;

namespace Entities;

public class InvoiceLine
{
    [Required]
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public double Value { get; set; }
    public string Description { get; set; } = default!;
    public string SchemeCode { get; set; } = default!;
    public string DeliveryBody { get; set; } = "RP00";

}
