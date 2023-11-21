using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.UserInvoicesCard;

public partial class UserInvoicesCard : ComponentBase
{
    [Parameter] public Invoice invoice { get; set; }
}