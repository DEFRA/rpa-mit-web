namespace EST.MIT.Web.Interfaces;

public interface INavigationStateContainer
{
    Task<string> GetBackUrlAsync();

    Task SetBackUrlAsync(string url);
}
