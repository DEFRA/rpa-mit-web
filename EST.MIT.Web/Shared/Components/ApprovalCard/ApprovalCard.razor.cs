using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.ApprovalCard;

public partial class ApprovalCard : ComponentBase
{
    [Parameter] public Invoice invoice { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }  

    public decimal TotalValueOfPaymentsGBP => invoice.PaymentRequests.Where(x => x.Currency == "GBP").Sum(x => x.Value);
    public decimal TotalValueOfPaymentsEU => invoice.PaymentRequests.Where(x => x.Currency == "EUR").Sum(x => x.Value);
    public string PaymentRequestsCurrencyGBP => invoice.PaymentRequests.Any(x => x.Currency == "GBP" && x.AgreementNumber != string.Empty) ? "GBR" : "";
    public string PaymentRequestsCurrencyEU => invoice.PaymentRequests.Any(x => x.Currency == "EUR") ? "EUR" : "";

    private string backUrl = "/approval/mine";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    private void View()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}/{WebUtility.UrlEncode(backUrl)}");
    }

    private void Approve()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/approval/confirm/approve");
    }

    private void Reject()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/approval/confirm/reject");
    }
}