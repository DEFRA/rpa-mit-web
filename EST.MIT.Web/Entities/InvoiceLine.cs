using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class InvoiceLine
{
    public Guid Id { get; set; }
    [Required]
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public double Value { get; set; }
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
