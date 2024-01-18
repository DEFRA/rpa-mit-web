using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;

namespace EST.MIT.Web.Pages.invoice.ViewInvoiceList;
public partial class ViewInvoiceList : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceAPI _API { get; set; }
    [Inject]
    private ITokenAcquisition TokenAcquisitionService { get; set; }
    
    [Inject]
    private MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

    private IEnumerable<Invoice> invoices = new List<Invoice>();

    [Inject]
    private IConfiguration configuration { get; set; }


    protected override async Task OnInitializedAsync()
    {
        try
        {
            var token = await TokenAcquisitionService.GetAccessTokenForUserAsync(new string[] { configuration.GetSection("MitWebApi").Value });

            invoices = await _API.GetInvoicesAsync(token);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            ConsentHandler.HandleException(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving invoices: {ex.Message}");
            _nav.NavigateTo("/error");
        }
    }
}