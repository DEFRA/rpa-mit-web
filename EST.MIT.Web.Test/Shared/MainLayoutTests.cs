using EST.MIT.Web.Test.FakeAuthentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace EST.MIT.Web.Test.Shared
{
    public class MainLayoutTests : TestContext
    {
        private readonly Mock<FakeAuthenticationStateProvider> _mockFakeAuthenticationProvider;

        public MainLayoutTests()
        {
            _mockFakeAuthenticationProvider = new Mock<FakeAuthenticationStateProvider>();
        }

        [Fact]
        public void MainLayout_Page_Shows_UserName()
        {
            //Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Henry Adetunji"),
                new Claim(ClaimTypes.Email,"henry.adetunji@defra.gov.uk"),
            };

            var authenticationStateProvider = FakeAuthenticationStateProvider.ThatReturnsClaims(claims);


            var authenticationState = _mockFakeAuthenticationProvider.Setup(x => x.GetAuthenticationStateAsync()).Returns(Task.FromResult(new AuthenticationState(authenticationStateProvider._principal)));
            Services.AddSingleton<AuthenticationStateProvider>(_mockFakeAuthenticationProvider.Object);

            //Act
            var component = RenderComponent<Web.Shared.MainLayout>();

            var value = component.FindAll("div.govuk-header__content")[0];

            var userName = value.TextContent;

            //Assert
            userName.Should().Be("Henry Adetunji");
        }
    }
}
