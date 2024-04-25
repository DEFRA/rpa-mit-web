using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.bulk.Confirmation;

public partial class Confirmation : ComponentBase
{
    [Parameter] public string ConfirmationNumber { get; set; } = string.Empty;
}