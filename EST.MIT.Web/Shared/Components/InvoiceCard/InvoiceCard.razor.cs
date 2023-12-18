using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace EST.MIT.Web.Shared.Components.InvoiceCard;

public partial class InvoiceCard : ComponentBase
{
    [Parameter] public Invoice invoice { get; set; }

    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    private readonly string backUrl = "/user-invoices";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    public void View()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}/{WebUtility.UrlEncode(backUrl)}");
    }
}