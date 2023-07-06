using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Approver
{
    [Required(ErrorMessage = "Please select an approver")]
    public string Name { get; set; } = default!;
}