using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputDouble;

public partial class GDSInputDouble : ComponentBase
{
    private double _data;
    private string _key = string.Empty;

    [Parameter]
    public double Data
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
    [Parameter] public EventCallback<double> DataChanged { get; set; }
    [Parameter] public Dictionary<string, string> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;

    private bool IsErrored { get; set; }
    private string err { get; set; } = default!;

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