using System.ComponentModel.DataAnnotations;

namespace Entities;
public class SearchCriteria
{
    [Required(ErrorMessage = "The Invoice Number field is required")]
    public string InvoiceNumber { get; set; } = "";


    [Required]
    public string Scheme { get; set; } = "";
}