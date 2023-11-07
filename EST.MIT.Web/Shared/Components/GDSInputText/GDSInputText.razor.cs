using Microsoft.AspNetCore.Components;
namespace EST.MIT.Web.Shared.Components.GDSInputText;

public partial class GDSInputText : ComponentBase
{
    private string _data = default!;
    private string _key = default!;
    private IEnumerable<string> _commonKeys = Enumerable.Empty<string>();

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

    [Parameter]
    public IEnumerable<string> CommonKeys
    {
        get => _commonKeys.Select(k => k.ToLower());
        set => _commonKeys = value?.Select(k => k.ToLower()) ?? Enumerable.Empty<string>();
    }

    [Parameter] public EventCallback<string> DataChanged { get; set; }
    [Parameter] public Dictionary<string, List<string>> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;

    private bool IsErrored { get; set; }
    private IEnumerable<string> err { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var errorKeys = CommonKeys.Any()
            ? Errors.Keys.Intersect(CommonKeys)
            : Enumerable.Empty<string>();

        errorKeys = errorKeys.Concat(new[] { Key });

        if (errorKeys.Any(k => Errors.ContainsKey(k)))
        {
            IsErrored = true;
            err = errorKeys
                .Where(k => Errors.ContainsKey(k))
                .SelectMany(k => Errors[k])
                .Distinct();  // This will ensure only unique error messages are considered
            return;
        }

        IsErrored = false;
    }

}