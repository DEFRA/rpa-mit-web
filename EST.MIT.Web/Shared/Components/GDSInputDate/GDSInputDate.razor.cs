using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputDate;

public partial class GDSInputDate : ComponentBase
{
    private DateTime _data = default!;
    private string _key = default!;
    private string DateDay { get; set; } = default!;
    private string DateMonth { get; set; } = default!;

    private string DateYear { get; set; } = default!;

    [Parameter]
    public DateTime Data
    {
        get => _data;
        set
        {
            if (_data != value)
            {
                _data = value;
                DateDay = value.Day.ToString();
                DateMonth = value.Month.ToString();
                DateYear = value.Year.ToString();
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
    [Parameter] public EventCallback<DateTime> DataChanged { get; set; }
    [Parameter] public Dictionary<string, List<string>> Errors { get; set; } = new();
    [Parameter] public string Label { get; set; } = default!;

    private bool IsErrored { get; set; }
    private IEnumerable<string> err { get; set; } = default!;

    private void UpdateData()
    {
        if (int.TryParse(DateDay, out int day) &&
            int.TryParse(DateMonth, out int month) &&
            int.TryParse(DateYear, out int year))
        {
            try
            {
                var newDate = new DateTime(year, month, day);
                if (newDate != Data) // Check to see if the date has actually changed
                {
                    Data = newDate;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Handle invalid date here
                IsErrored = true;
                err = err.Concat(new[] { $"The value for {Key} is invalid" });
            }
        }
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