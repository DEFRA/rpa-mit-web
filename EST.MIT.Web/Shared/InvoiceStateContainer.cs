using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Shared;

public interface IInvoiceStateContainer
{
    Invoice? Value { get; }
    event Action OnStateChange;
    void SetValue(Invoice? value);
}

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