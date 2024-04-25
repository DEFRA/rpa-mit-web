using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputSelect;

public partial class GDSInputSelect : ComponentBase
{
    private string _data = default!;
    private string _key = default!;

    [Parameter]
    public string Key
    {
        get => _key?.ToLower();
        set => _key = value.ToLower();
    }

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

    [Parameter] public EventCallback<string> DataChanged { get; set; }
    [Parameter] public Dictionary<string, List<string>> Errors { get; set; } = new();
    [Parameter] public Dictionary<string, string> Options { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;
    [Parameter] public bool HasChooseOption { get; set; } = false;

    private bool IsErrored { get; set; }
    private IEnumerable<string> err { get; set; } = default!;


    public GDSInputSelect()
    {
        this.err = new List<string>();
    }

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