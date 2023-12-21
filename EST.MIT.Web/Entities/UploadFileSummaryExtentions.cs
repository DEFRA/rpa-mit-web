using System.Text.Json;

namespace EST.MIT.Web.Entities;

public static class UploadFileSummaryExtentions
{
    public static string ToMessage(this ImportRequestSummary entity) => JsonSerializer.Serialize<ImportRequestSummary>(entity);
}
