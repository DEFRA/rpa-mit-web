
using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.UserUploadsCard;

public partial class UserUploadsCard : ComponentBase
{
    [Parameter] public IEnumerable<ImportRequest> importRequests { get; set; }

    static string FormatTimestamp(DateTimeOffset? timestamp)
    {
        if (timestamp == null)
        {
            return "N/A";
        }
        return timestamp.Value.ToString("dd/MM/yyyy");
    }
}