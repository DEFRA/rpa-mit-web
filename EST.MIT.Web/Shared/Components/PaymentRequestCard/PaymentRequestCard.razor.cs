using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.ViewInvoiceSummary;

namespace EST.MIT.Web.Shared.Components.PaymentRequestCard;

public partial class PaymentRequestCard : ComponentBase
{
    [CascadingParameter] public ViewInvoiceSummary _Parent { get; set; }
    [Parameter] public PaymentRequest PaymentRequest { get; set; }
    [Parameter] public string AccountType { get; set; }
    [Parameter] public bool ReadOnly { get; set; } = false;
}