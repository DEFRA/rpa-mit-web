using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared.Components.InvoiceMetaData;

public partial class InvoiceMetaData : ComponentBase
{
    [Parameter] public Invoice Invoice { get; set; } = new();
    [Parameter] public string Class { get; set; }
}