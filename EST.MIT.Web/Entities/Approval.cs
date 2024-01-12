using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class Approval
{
    [Required(ErrorMessage = "You must provide a reason for rejection")]
    public string Reason { get; set; }
}