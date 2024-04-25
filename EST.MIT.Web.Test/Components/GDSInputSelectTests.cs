using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared.Components.GDSInputSelect;

namespace EST.MIT.Web.Tests.Components;

public class GDSInputSelectTests : TestContext
{
    [Fact]
    public void GDSInputSelect_Parameters_Are_Set_To_Error()
    {
        var errors = new Dictionary<string, List<string>>
        {
            { "testkey", new List<string>() { "TestError" } }
        };

        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSInputSelect>(parameters =>
        {
            parameters.Add(p => p.Options, new Dictionary<string, string>() { { "Option 1", "Option1" }, { "Option 2", "Option2" } });
            parameters.Add(p => p.Data, "Option1");
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
            parameters.Add(p => p.Errors, errors);
        });

        var InputErrorClass = component.FindAll(".govuk-select--error");

        component.Instance.Errors.Should().NotBeEmpty();
        component.Instance.Data.Should().Be("Option1");
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        component.Instance.Options.Count.Should().Be(2);
        component.Instance.Options.First().Key.Should().Be("Option 1");
        component.Instance.Options.First().Value.Should().Be("Option1");
        component.Instance.Options.Last().Key.Should().Be("Option 2");
        component.Instance.Options.Last().Value.Should().Be("Option2");
        InputErrorClass.Should().NotBeEmpty();
    }

    [Fact]
    public void GDSInputSelect_Parameters_Are_Set_To_Validate()
    {
        RenderTree.Add<CascadingValue<EditContext>>(parameters =>
        {
            parameters.Add(p => p.Value, new EditContext(new PaymentRequest()));
        });

        var component = RenderComponent<GDSInputSelect>(parameters =>
        {
            parameters.Add(p => p.Options, new Dictionary<string, string>() { { "Option 1", "Option1" }, { "Option 2", "Option2" } });
            parameters.Add(p => p.Data, "Option1");
            parameters.Add(p => p.Label, "TestLabel");
            parameters.Add(p => p.Key, "TestKey");
        });

        var InputErrorClass = component.FindAll(".govuk-select--error");

        component.Instance.Errors.Should().BeEmpty();
        component.Instance.Data.Should().Be("Option1");
        component.Instance.Label.Should().Be("TestLabel");
        component.Instance.Key.Should().Be("testkey");
        component.Instance.Options.Count.Should().Be(2);
        component.Instance.Options.First().Key.Should().Be("Option 1");
        component.Instance.Options.First().Value.Should().Be("Option1");
        component.Instance.Options.Last().Key.Should().Be("Option 2");
        component.Instance.Options.Last().Value.Should().Be("Option2");
        InputErrorClass.Should().BeEmpty();
    }
}