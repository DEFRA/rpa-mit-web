namespace EST.MIT.Web.Helpers
{
    public static class ErrorMessageHelper
    {
        public static string ErrorMessagesForField(Dictionary<string, List<string>> errors, string fieldKey)
        {
            if (errors.Any(x => x.Key.ToLower().Contains(fieldKey.ToLower())))
            {
                var values = errors.FirstOrDefault(x => x.Key.ToLower().Contains(fieldKey.ToLower())).Value;

                if (values.Count > 0)
                {
                    return values[0];
                }
            }
            return null;
        }
    }
}
