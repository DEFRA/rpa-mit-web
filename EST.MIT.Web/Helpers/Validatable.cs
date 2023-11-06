namespace EST.MIT.Web.Helpers;

public class Validatable : IValidatable
{
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
    private string _errorPath = string.Empty;
    public string ErrorPath
    {
        get { return _errorPath ?? string.Empty; }
        set { _errorPath = value; }
    }
    public virtual Dictionary<string, List<string>> AddErrors(Dictionary<string, List<string>> errors)
    {
        Errors.Clear();
        foreach (var error in errors)
        {
            if (error.Key.StartsWith(ErrorPath))
            {
                string errorKey = error.Key.Remove(0, ErrorPath.Length);
                if (!errorKey.Contains('.') &&
                    !Errors.ContainsKey(errorKey))
                {
                    Errors.Add(errorKey, error.Value);
                }
            }
        }
        return Errors;
    }
}
