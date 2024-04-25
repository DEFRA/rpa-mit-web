using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IFindService
{
    Task<Invoice> FindInvoiceAsync(SearchCriteria criteria);
}
