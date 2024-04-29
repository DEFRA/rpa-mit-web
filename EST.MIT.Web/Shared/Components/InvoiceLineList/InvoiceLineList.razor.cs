using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared.Components.InvoiceLineList;

public partial class InvoiceLineList : ComponentBase
{
    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}