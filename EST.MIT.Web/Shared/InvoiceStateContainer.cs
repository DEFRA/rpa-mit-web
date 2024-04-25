using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Shared;

public class InvoiceStateContainer : IInvoiceStateContainer
{
    public Invoice? Value { get; set; }
    public event Action? OnStateChange;
    public void SetValue(Invoice? value)
    {
        Value = value;
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnStateChange?.Invoke();
}