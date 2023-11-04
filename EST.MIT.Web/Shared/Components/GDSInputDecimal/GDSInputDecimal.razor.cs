using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputDecimal;

public partial class GDSInputDecimal : ComponentBase
{
    private decimal _data;
    private string _key = string.Empty;

    [Parameter]
    public decimal Data
    {
        get => _data;
        set
        {
            if (value != _data)
            {
                _data = value;
                DataChanged.InvokeAsync(value);
            }
        }
    }
    [Parameter]
    public string Key
    {
        get => _key.ToLower();
        set => _key = value.ToLower();
    }
    [Parameter] public EventCallback<decimal> DataChanged { get; set; }
    [Parameter] public Dictionary<string, List<string>> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;

    private bool IsErrored { get; set; }
    private IEnumerable<string> err { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Errors.ContainsKey(Key))
        {
            IsErrored = true;
            err = Errors[Key];
            return;
        }

        IsErrored = false;
    }
}