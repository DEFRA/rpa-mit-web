using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class AccountSelect
{
    [Required(ErrorMessage = "Please select an account type")]
    public string Account { get; set; } = default!;
}
