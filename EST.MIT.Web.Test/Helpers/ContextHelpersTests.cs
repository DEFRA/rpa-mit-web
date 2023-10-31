using Microsoft.AspNetCore.Http;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Tests.Helpers;
public class ContextHelpersTests
{
    [Fact]
    public void GetBaseURI_Returns_String()
    {

        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost");

        context.GetBaseURI().Should().Be("http://localhost");
    }
}