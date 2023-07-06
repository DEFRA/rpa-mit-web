using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.GDSInputRadio;

public partial class GDSInputRadio : ComponentBase
{
    [Parameter] public IEnumerable<KeyValuePair<string, string>> Options { get; set; } = default!;
    [Parameter] public bool ShowDescriptions { get; set; } = false;
    [Parameter] public bool UseKeys { get; set; } = true;
}