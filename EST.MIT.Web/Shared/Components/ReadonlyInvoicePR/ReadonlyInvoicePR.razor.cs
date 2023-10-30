using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared.Components.ReadonlyInvoicePR;

public partial class ReadonlyInvoicePR : ComponentBase
{
    [Parameter] public Invoice Invoice { get; set; } = default!;

    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}