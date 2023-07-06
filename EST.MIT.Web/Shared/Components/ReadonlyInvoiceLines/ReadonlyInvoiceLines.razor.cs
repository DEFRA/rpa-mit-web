using Microsoft.AspNetCore.Components;
using Entities;

namespace EST.MIT.Web.Shared.Components.ReadonlyInvoiceLines;

public partial class ReadonlyInvoiceLines : ComponentBase
{
    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}