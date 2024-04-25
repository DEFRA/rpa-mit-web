using EST.MIT.Web.Pages.bulk.Confirmation;

namespace EST.MIT.Web.Tests.Pages;

public class ConfirmationPageTests : TestContext
{

    [Fact]
    public void Confirmation_Route_Value_Is_Displayed()
    {
        IRenderedComponent<Confirmation> component = RenderComponent<Confirmation>();
        component.SetParametersAndRender(parameters => parameters.Add(p => p.ConfirmationNumber, "1234567890"));

        var panel = component.FindAll(".govuk-panel__body");

        panel[0].InnerHtml.Should().Contain("Your reference number<br><strong>1234567890</strong>");
    }

}