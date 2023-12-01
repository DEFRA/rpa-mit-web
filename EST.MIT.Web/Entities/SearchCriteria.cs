using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.Entities;

[ExcludeFromCodeCoverage]
public class SearchCriteria
{
    [Required(ErrorMessage = "The Invoice Number field is required")]
    public string InvoiceNumber { get; set; } = "";
}