using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputInt;

public partial class GDSInputInt : ComponentBase
{
    private int _data;
    private string _key = string.Empty;

    [Parameter]
    public int Data
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
    [Parameter] public EventCallback<int> DataChanged { get; set; }
    [Parameter] public Dictionary<string, List<string>> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;

    private bool IsErrored { get; set; }
    private IEnumerable<string> err { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (Errors.Any())
        {
            if (Errors.ContainsKey(Key))
            {
                IsErrored = true;
                err = Errors[Key];
                return;
            }
        }

        IsErrored = false;
    }
}