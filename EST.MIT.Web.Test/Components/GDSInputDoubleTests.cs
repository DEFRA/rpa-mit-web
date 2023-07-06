using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Entities;
using EST.MIT.Web.Shared.Components.GDSInputDouble;

namespace Components.Tests;

public class GDSInputDoubleTests : TestContext
{
    [Fact]
    public void GDSInputDouble_Parameters_Are_Set_To_Error()
    {
        var errors = new Dictionary<string, string>();
        errors.Add("testkey", "TestError");

        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSInputDouble>(parameters =>
        {
            parameters.Add(p => p.Data, 123.45);
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
            parameters.Add(p => p.Errors, errors);
        });

        var InputErrorClass = component.FindAll(".govuk-input--error");

        component.Instance.Errors.Should().NotBeEmpty();
        component.Instance.Data.Should().Be(123.45);
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        InputErrorClass.Should().NotBeEmpty();
    }

    [Fact]
    public void GDSInputDouble_Parameters_Are_Set_To_Validate()
    {
        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSInputDouble>(parameters =>
        {
            parameters.Add(p => p.Data, 123.45);
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
        });

        var InputErrorClass = component.FindAll(".govuk-input--error");

        component.Instance.Errors.Should().BeEmpty();
        component.Instance.Data.Should().Be(123.45);
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        InputErrorClass.Should().BeEmpty();
    }
}