using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;


namespace EST.MIT.Web.Test.FakeAuthentication
{
    public class FakeAuthenticationStateProvider : AuthenticationStateProvider
    {
        public ClaimsPrincipal _principal;

        public FakeAuthenticationStateProvider()
        {
            _principal = new ClaimsPrincipal();
        }

        public FakeAuthenticationStateProvider(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        public static FakeAuthenticationStateProvider ThatReturnsClaims(params Claim[] claims)
        {
            string? authType = "Token";
            var identity = new ClaimsIdentity(claims, authType);
            var principal = new ClaimsPrincipal(identity);
            return new FakeAuthenticationStateProvider(principal);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_principal));
        }
    }
}
