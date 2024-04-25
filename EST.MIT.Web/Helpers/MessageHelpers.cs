using System.Text.Json;

namespace EST.MIT.Web.Helpers;

public static class QueueHandlers
{
    public static string ToMessage<T>(this T entity) => JsonSerializer.Serialize<T>(entity);
}