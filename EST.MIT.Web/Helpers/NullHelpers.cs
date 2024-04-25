namespace EST.MIT.Web.Helpers;

public static class NullHandlers
{
    public static bool IsNull(this object target)
    {
        if (target != null)
            return false;

        return true;
    }
}