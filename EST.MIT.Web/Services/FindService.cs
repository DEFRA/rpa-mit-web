using EST.MIT.Web.Helpers;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class FindService : IFindService
{
    private readonly IInvoiceAPI _apiService;
    private readonly IReferenceDataRepository _referenceDataRepository;

    public FindService(IInvoiceAPI apiService, IReferenceDataRepository referenceDataRepository)
    {
        _apiService = apiService;
        _referenceDataRepository = referenceDataRepository;
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
