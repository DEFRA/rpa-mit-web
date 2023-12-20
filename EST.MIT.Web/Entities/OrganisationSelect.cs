using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public class OrganisationSelect
{
    [Required(ErrorMessage = "Please select an organisation")]
    public string Organisation { get; set; } = default!;
}
