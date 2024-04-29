using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EST.MIT.Web.Shared;

public class NavigationStateContainer : INavigationStateContainer
{


    private readonly NavigationManager _navigationManager;
    private readonly ProtectedSessionStorage _protectedSessionStorage;

    public NavigationStateContainer(NavigationManager navigationManager, ProtectedSessionStorage protectedSessionStorage)
    {
        _navigationManager = navigationManager;
        _protectedSessionStorage = protectedSessionStorage;
    }

    public async Task<string> GetBackUrlAsync()
    {
        var result = await _protectedSessionStorage.GetAsync<string>("BackUrl");
        return result.Success ? result.Value : "/";
    }

    public async Task SetBackUrlAsync(string url)
    {
        if (this.IsUrlFromAppDomain(url))
        {
            await _protectedSessionStorage.SetAsync("BackUrl", url);
        }
    }

    private bool IsUrlFromAppDomain(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        // For relative URLs, assume they are safe
        if (Uri.IsWellFormedUriString(url, UriKind.Relative))
        {
            return true;
        }

        try
        {
            var uri = new Uri(url, UriKind.Absolute);
            var baseUri = new Uri(_navigationManager.BaseUri);

            return uri.Host.Equals(baseUri.Host, StringComparison.OrdinalIgnoreCase);
        }
        catch (UriFormatException)
        {
            // Log or handle the exception as needed
            return false;
        }
    }
}