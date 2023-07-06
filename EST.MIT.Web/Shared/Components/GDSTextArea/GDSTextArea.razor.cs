using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSTextArea;

public partial class GDSTextArea : ComponentBase
{
    private string _data = default!;
    private string _key = string.Empty;

    [Parameter]
    public string Data
    {
        get => _data;
        set
        {
            if (_data != value)
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
    [Parameter] public EventCallback<string> DataChanged { get; set; }
    [Parameter] public Dictionary<string, string> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;
    [Parameter] public string Hint { get; set; } = default!;

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

