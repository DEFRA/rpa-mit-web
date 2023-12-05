using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace EST.MIT.Web.Shared.Components.UserInvoicesCard;

public partial class UserInvoicesCard : ComponentBase
{
    [Parameter] public Invoice invoice { get; set; }

    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    public int NumberOfPaymentRequests => invoice.PaymentRequests.Count;
    public decimal TotalValueOfPaymentsGBP => invoice.PaymentRequests.Where(x => x.Currency == "GBP").Sum(x => x.Value);
    public decimal TotalValueOfPaymentsEUR => invoice.PaymentRequests.Where(x => x.Currency == "EUR").Sum(x => x.Value);
    public string PaymentRequestsCurrencyGBP => invoice.PaymentRequests.Any(x => x.Currency == "GBP" && x.AgreementNumber != string.Empty) ? "GBP" : "";
    public string PaymentRequestsCurrencyEUR => invoice.PaymentRequests.Any(x => x.Currency == "EUR") ? "EUR" : "";

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