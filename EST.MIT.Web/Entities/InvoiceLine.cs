using System.ComponentModel.DataAnnotations;

namespace Entities;

public class InvoiceLine
{
    [Required]
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public double Value { get; set; }
    public string FundCode { get; set; }
    public string MainAccount { get; set; }
    public string SchemeCode { get; set; }
    [Range(2014, Double.MaxValue, ErrorMessage = "The Marketing Year must be greater than 2014")]
    public int MarketingYear { get; set; }
    public string DeliveryBody { get; set; } = "RP00";
    public string Description { get; set; } = default!;
    public string DebtType { get; set; } = default!;

}
