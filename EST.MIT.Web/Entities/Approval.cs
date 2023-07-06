using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Approval
{
    [Required(ErrorMessage = "You must provide a justification")]
    public string Justification { get; set; }
}