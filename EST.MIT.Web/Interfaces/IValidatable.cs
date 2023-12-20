namespace EST.MIT.Web.Interfaces;

public interface IValidatable
{
    Dictionary<string, List<string>> Errors { get; set; }
    Dictionary<string, List<string>> AllErrors { get; }
}
