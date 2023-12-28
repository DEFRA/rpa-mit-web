using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class SchemeSelect
{
    [Required(ErrorMessage = "Please select a scheme type")]
    public string Scheme { get; set; } = default!;
}
