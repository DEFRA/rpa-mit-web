using Helpers;
using Entities;

namespace Services;

public interface IFindService
{
    Task<Invoice> FetchInvoiceAsync(string invoiceNumber, string scheme);
}

public class FindService : IFindService
{
    private readonly IInvoiceAPI _apiService;

    public FindService(IInvoiceAPI ApiService)
    {
        _apiService = ApiService;
    }

    public async Task<Invoice> FetchInvoiceAsync(string invoiceNumber, string scheme)
    {

        var invoice = await _apiService.FindInvoiceAsync(invoiceNumber, scheme);
        if (invoice.IsNull())
        {
            return null;
        }
        return invoice;
    }

}
