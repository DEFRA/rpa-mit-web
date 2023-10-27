using Microsoft.AspNetCore.Components;
using Entities;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.Summary;

namespace EST.MIT.Web.Shared.Components.SummaryPaymentRequest;

public partial class SummaryPaymentRequest : ComponentBase
{
    [CascadingParameter] public Summary _Parent { get; set; }
    [Parameter] public PaymentRequest PaymentRequest { get; set; }
    [Parameter] public string AccountType { get; set; }
    [Parameter] public bool HideActions { get; set; } = false;
}