using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared.Components.GDSTextArea;

namespace EST.MIT.Web.Tests.Components;

public class GDSTextAreaTests : TestContext
{
    [Fact]
    public void GDSTextArea_Parameters_Are_Set_To_Error()
    {
        var errors = new Dictionary<string, List<string>>
        {
            { "testkey", new List<string>() { "TestError" } }
        };

        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSTextArea>(parameters =>
        {
            parameters.Add(p => p.Data, "123");
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
            parameters.Add(p => p.Errors, errors);
        });

        var InputErrorClass = component.FindAll(".govuk-textarea--error");

        component.Instance.Errors.Should().NotBeEmpty();
        component.Instance.Data.Should().Be("123");
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        InputErrorClass.Should().NotBeEmpty();
    }

    [Fact]
    public void GDSTextArea_Parameters_Are_Set_To_Validate()
    {
        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSTextArea>(parameters =>
        {
            parameters.Add(p => p.Data, "123");
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
        });

        var InputErrorClass = component.FindAll(".govuk-textarea--error");

        component.Instance.Errors.Should().BeEmpty();
        component.Instance.Data.Should().Be("123");
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        InputErrorClass.Should().BeEmpty();
    }
}