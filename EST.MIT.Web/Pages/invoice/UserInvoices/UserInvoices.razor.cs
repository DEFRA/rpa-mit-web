using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;
using System;

namespace EST.MIT.Web.Pages.invoice.UserInvoices;
public partial class UserInvoices : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceAPI _API { get; set; }

    [Inject] Microsoft.Identity.Web.ITokenAcquisition TokenAcquisitionService { get; set; }


    private IEnumerable<Invoice> invoices = new List<Invoice>();


    protected override async Task OnInitializedAsync()
    {
        try
        {
            var token = await TokenAcquisitionService.GetAccessTokenForUserAsync(new string[] { $"https://graph.microsoft.com/.default" });


            invoices = await _API.GetInvoicesAsync();
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error retrieving invoices: {ex.Message}");
            _nav.NavigateTo("/error");
        }
    }
}