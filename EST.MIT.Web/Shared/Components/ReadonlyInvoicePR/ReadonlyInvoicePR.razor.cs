using Microsoft.AspNetCore.Components;
using Entities;

namespace EST.MIT.Web.Shared.Components.ReadonlyInvoicePR;

public partial class ReadonlyInvoicePR : ComponentBase
{
    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}