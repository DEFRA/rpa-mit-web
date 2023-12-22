using System.Text.Json;

namespace EST.MIT.Web.Entities;

public static class ImportRequestExtentions
{
    public static string ToMessage(this ImportRequest entity) => JsonSerializer.Serialize<ImportRequest>(entity);
}
