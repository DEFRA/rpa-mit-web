using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.approvals.ApprovalRejectionConfirmation;

public partial class ApprovalRejectionConfirmation : ComponentBase
{
    [Parameter] public string Id { get; set; }
    [Parameter] public string ApprovalType { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private bool IsRejectEndpoint
    {
        get
        {
            return (ApprovalType is not null) && ApprovalType.ToLower() == "rejected";
        }
    }
}