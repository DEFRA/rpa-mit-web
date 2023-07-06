using System.Text.Json;

namespace Helpers;

public static class QueueHandlers
{
    public static string ToMessage<T>(this T entity) => JsonSerializer.Serialize<T>(entity);
}