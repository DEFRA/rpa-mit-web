using EST.MIT.Web.Helpers;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class FindService : IFindService
{
    private readonly IInvoiceAPI _apiService;

    public FindService(IInvoiceAPI apiService)
    {
        _apiService = apiService;
    }

    public async Task<Invoice> FindInvoiceAsync(SearchCriteria criteria)
    {

        var invoice = await _apiService.FindInvoiceAsync(criteria);
        if (invoice.IsNull())
        {
            return null;
        }
        return invoice;
    }
}
