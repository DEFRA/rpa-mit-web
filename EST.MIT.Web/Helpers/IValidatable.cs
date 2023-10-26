namespace EST.MIT.Web.Helpers;

public interface IValidatable
{
    Dictionary<string, string> Errors { get; set; }
}

public class Validatable : IValidatable
{
    public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    public string ErrorPath { get; set; }
    public virtual Dictionary<string, string> AddErrors(Dictionary<string, string> errors)
    {
        Errors.Clear();
        foreach (var error in errors)
        {
            if (error.Key.StartsWith(ErrorPath))
            {
                string errorKey = error.Key.Remove(0, ErrorPath.Length);
                if (!errorKey.Contains('.') && errorKey.Length > 0 &&
                    !Errors.ContainsKey(errorKey))
                {
                    Errors.Add(errorKey, error.Value);
                }
            }
        }
        return Errors;
    }
}
