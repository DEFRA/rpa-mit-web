namespace EST.MIT.Web.Helpers;

public interface IValidatable
{
    Dictionary<string, List<string>> Errors { get; set; }
}
