namespace EST.MIT.Web.Helpers;

public static class ContextHelpers
{
    public static string GetBaseURI(this HttpContext? context)
    {
        if (context == null)
            return string.Empty;

        var request = context.Request;
        var host = request.Host;
        var scheme = request.Scheme;

        return $"{scheme}://{host}";
    }
}