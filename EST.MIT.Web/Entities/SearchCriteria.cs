using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Entities;

[ExcludeFromCodeCoverageAttribute]
public class SearchCriteria
{
    [Required(ErrorMessage = "The Invoice Number field is required")]
    public string InvoiceNumber { get; set; } = "";


    [Required]
    public string Scheme { get; set; } = "";
}