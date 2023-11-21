using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.create_invoice.CreateInvoice;

public partial class CreateInvoice : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    private void Start()
    {
        _invoiceStateContainer.SetValue(new Invoice());
        _nav.NavigateTo("/create-invoice/account");
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}