namespace EST.MIT.Web.Interfaces;

public interface IPageServices
{
    bool Validation(dynamic model, out bool IsErrored, out Dictionary<string, List<string>> errors);
}

