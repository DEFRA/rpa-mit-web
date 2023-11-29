namespace EST.MIT.Web.Test.Pages;

public class ErrorTests : TestContext
{
    [Fact]
    public void Error_Page_RendersSuccessfully()
    {
        var component = RenderComponent<EST.MIT.Web.Pages.Error>();

        var title = component.FindAll("h1.govuk-heading-l")[0];
        var paragraphA = component.FindAll("p.govuk-body")[0];
        var paragraphB = component.FindAll("p.govuk-body")[1];
        var paragraphC = component.FindAll("p.govuk-body")[2];

        title.InnerHtml.Should().Be("Sorry, there is a problem with the service");
        paragraphA.InnerHtml.Should().Be("The service may be experiencing temporary difficulties.");
        paragraphB.InnerHtml.Should().Be("Try again later.");
        paragraphC.InnerHtml.Should().Be("<a class=\"govuk-link\" href=\"#\">Contact the Support team</a> If the issue persists, please reach out to MIT Support for further assistance.");

    }

}
