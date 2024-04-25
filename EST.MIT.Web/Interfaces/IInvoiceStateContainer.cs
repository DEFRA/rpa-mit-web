using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IInvoiceStateContainer
{
    Invoice? Value { get; }
    event Action OnStateChange;
    void SetValue(Invoice? value);
}
