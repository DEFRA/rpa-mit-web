using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.AddInvoiceLine;

public partial class AddInvoiceLine : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = "";

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private readonly InvoiceLine invoiceLine = new();
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private readonly Dictionary<string, string> allFundCodes = new();
    private readonly Dictionary<string, string> allAccounts = new();
    private readonly Dictionary<string, string> allSchemeCodes = new();
    private readonly Dictionary<string, string> allMarketingYears = new();
    private readonly Dictionary<string, string> allDeliveryBodies = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice ??= _invoiceStateContainer.Value;
        paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);

        await _referenceDataAPI.GetAccountsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType).ContinueWith(x =>
        {
            if (x.Result.IsSuccess)
            {
                foreach (var account in x.Result.Data)
                {
                    allAccounts.Add(account.code, account.description);
                }
            }
        });

        await _referenceDataAPI.GetDeliveryBodiesAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType).ContinueWith(x =>
        {
            if (x.Result.IsSuccess)
            {
                foreach (var deliveryBody in x.Result.Data)
                {
                    allDeliveryBodies.Add(deliveryBody.code, deliveryBody.description);
                }
            }
        });

        await _referenceDataAPI.GetSchemesAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType).ContinueWith(x =>
        {
            if (x.Result.IsSuccess)
            {
                foreach (var scheme in x.Result.Data)
                {
                    allSchemeCodes.Add(scheme.code, scheme.description);
                }
            }
        });

        await _referenceDataAPI.GetMarketingYearsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType).ContinueWith(x =>
        {
            if (x.Result.IsSuccess)
            {
                foreach (var marketingYear in x.Result.Data)
                {
                    allMarketingYears.Add(marketingYear.code, marketingYear.description);
                }
            }
        });

        await _referenceDataAPI.GetFundsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType).ContinueWith(x =>
        {
            if (x.Result.IsSuccess)
            {
                foreach (var fund in x.Result.Data)
                {
                    allFundCodes.Add(fund.code, fund.description);
                }
            }
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (paymentRequest.IsNull() || invoice.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }

    private string ErrorMessagesForField(string fieldKey)
    {
        return ErrorMessageHelper.ErrorMessagesForField(errors, fieldKey);
    }

    private async Task SaveInvoiceLine()
    {
        if (!_pageServices.Validation(invoiceLine, out IsErrored, out errors)) return;

        var response = await _api.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine);

        if (response.IsSuccess)
        {
            IsErrored = false;
            errors.Clear();
            _invoiceStateContainer.SetValue(response.Data);
            _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
        }
        else
        {
            IsErrored = true;
            errors = invoiceLine.Errors;
        }
    }

    private void Cancel()
    {
        _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
    }
}