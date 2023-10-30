using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared.Components.ReadonlyInvoiceLines;

public partial class ReadonlyInvoiceLines : ComponentBase
{
    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}