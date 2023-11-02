using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputDate;

public partial class GDSInputDate : ComponentBase
{
    private DateTime _data = default!;
    private string _key = default!;

    private string _dateDay = default!;
    private string _dateMonth = default!;
    private string _dateYear = default!;


    [Parameter]
    public string Key
    {
        get => _key?.ToLower();
        set => _key = value.ToLower();
    }

    [Parameter]
    public DateTime Data
    {
        get => _data;
        set
        {
            if (_data != value)
            {
                _data = value;

                if (DateDay != value.Day.ToString())
                {
                    DateDay = value.Day.ToString();
                }

                if (DateMonth != value.Month.ToString())
                {
                    DateMonth = value.Month.ToString();
                }

                if (DateYear != value.Year.ToString())
                {
                    DateYear = value.Year.ToString();
                }

                DataChanged.InvokeAsync(value);
            }
        }
    }

    public string DateDay
    {
        get => _dateDay;
        set
        {
            if (_dateDay != value)
            {
                _dateDay = value;
                this.UpdateData();
            }
        }
    }

    public string DateMonth
    {
        get => _dateMonth;
        set
        {
            if (_dateMonth != value)
            {
                _dateMonth = value;
                this.UpdateData();
            }
        }
    }

    public string DateYear
    {
        get => _dateYear;
        set
        {
            if (_dateYear != value)
            {
                _dateYear = value;
                this.UpdateData();
            }
        }
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
                DateTime newDate;

                if (DateTime.TryParse(string.Format("{0}-{1}-{2}", year, month, day), out newDate))
                {
                    // Date was valid.
                    if (newDate != Data) // Check to see if the date has actually changed
                    {
                        Data = newDate;
                    }
                }
                else
                {
                    // Handle invalid date here
                    IsErrored = true;
                    err = err.Concat(new[] { $"The value for {Label} is invalid" });
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Handle invalid date here
                IsErrored = true;
                err = err.Concat(new[] { $"The value for {Label} is invalid" });
            }
        }
    }

    public GDSInputDate()
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