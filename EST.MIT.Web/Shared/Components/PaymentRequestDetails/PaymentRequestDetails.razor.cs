using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared.Components.PaymentRequestDetails;

public partial class PaymentRequestDetails : ComponentBase
{
    [Parameter] public Invoice Invoice { get; set; } = default!;

    [Parameter] public PaymentRequest PaymentRequest { get; set; } = default!;
}