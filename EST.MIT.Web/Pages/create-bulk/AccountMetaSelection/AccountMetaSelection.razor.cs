using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.create_bulk.AccountMetaSelection;

public partial class AccountMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; }
    [Inject] private ILogger<AccountMetaSelection> Logger { get; set; }

    private Invoice invoice = default!;
    private AccountSelect accountSelect = new();
    readonly Dictionary<string, string> accountTypes = new()
    {
        { "AR", "Accounts Receivable" },
        { "AP", "Accounts Payable" }
    };
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();

    protected override void OnInitialized()
    {
        try
        {
            base.OnInitialized();
            invoice = _invoiceStateContainer.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing AccountMetaSelection page");
            _nav.NavigateTo("/error");
        }

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (_invoiceStateContainer.Value == null || _invoiceStateContainer.Value.IsNull())
            {
                _invoiceStateContainer.SetValue(null);
                _nav.NavigateTo("/create-bulk");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnAfterRenderAsync of AccountMetaSelection page");
            _nav.NavigateTo("/error");
        }
    }

    private void SaveAndContinue()
    {
        try
        {
            invoice.AccountType = accountSelect.Account;
            _invoiceStateContainer.SetValue(invoice);
            _nav.NavigateTo("/create-bulk/organisation");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in SaveAndContinue of AccountMetaSelection page");
            _nav.NavigateTo("/error");
        }
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(accountSelect, out IsErrored, out errors);
        viewErrors = errors[nameof(accountSelect.Account).ToLower()];
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}