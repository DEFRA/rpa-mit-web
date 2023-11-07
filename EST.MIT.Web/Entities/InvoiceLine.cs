using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class InvoiceLine : Validatable
{
    public Guid Id { get; set; }
    [Required]
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public double Value { get; set; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = default!;
    [Required(ErrorMessage = "Fund code is required")]
    public string FundCode { get; set; } = "";
    [Required(ErrorMessage = "Main account is required")]
    public string MainAccount { get; set; } = "";
    [Required(ErrorMessage = "Scheme code is required")]
    public string SchemeCode { get; set; } = "";
    [Required(ErrorMessage = "Marketing year is required")]
    public string MarketingYear { get; set; } = "";
    [Required(ErrorMessage = "Delivery body is required")]
    public string DeliveryBody { get; set; } = "";
}
