namespace EST.MIT.Web.Test.Pages;

public class IndexTests : TestContext
{
    [Fact]
    public void CreateNewInvoice_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();

        var title = component.FindAll("h2.govuk-heading-l")[0];
        var heading = component.FindAll("p.govuk-body")[0];

        title.InnerHtml.Should().Be("Manual Invoice Service");
        heading.InnerHtml.Should().Be("Tools to create and manage manual payments across DEFRA managed schemes");
    }

    [Fact]
    public void Index_Sections_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();
        var chevronCards = component.FindAll("div.chevron-card__wrapper");
        var chevronCardLinks = component.FindAll("a.chevron-card__link");
        var chevronCardParagraphs = component.FindAll("p.chevron-card__description");

        Assert.Equal(6, chevronCards.Count);
        Assert.Equal(6, chevronCardLinks.Count);
        Assert.Equal(6, chevronCardParagraphs.Count);
    }

    [Fact]
    public void FindInvoice_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();
        var chevronCardLink = component.FindAll("a.chevron-card__link")[2];
        var chevronCardParagraph = component.FindAll("p.chevron-card__description")[2];

        Assert.Equal("Search for an invoice by invoice numbers.", chevronCardParagraph.InnerHtml.Trim());
        Assert.Equal("Find an Invoice", chevronCardLink.InnerHtml);
        Assert.Equal("/find", chevronCardLink.GetAttribute("href"));
    }

    [Fact]
    public void MyInvoices_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();
        var chevronCardLink = component.FindAll("a.chevron-card__link")[3];
        var chevronCardParagraph = component.FindAll("p.chevron-card__description")[3];

        Assert.Equal("Show invoices created by you.", chevronCardParagraph.InnerHtml.Trim());
        Assert.Equal("My Invoices", chevronCardLink.InnerHtml);
        Assert.Equal("/user-invoices", chevronCardLink.GetAttribute("href"));
    }

    [Fact]
    public void MyApprovals_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();
        var chevronCardLink = component.FindAll("a.chevron-card__link")[4];
        var chevronCardParagraph = component.FindAll("p.chevron-card__description")[4];

        Assert.Equal("Show invoices awaiting your approval.", chevronCardParagraph.InnerHtml.Trim());
        Assert.Equal("My Approvals", chevronCardLink.InnerHtml);
        Assert.Equal("user-approvals", chevronCardLink.GetAttribute("href"));
    }

    [Fact]
    public void MyUploads_RendersSuccessfully()
    {
        var component = RenderComponent<Web.Pages.Index>();
        var chevronCardLink = component.FindAll("a.chevron-card__link")[5];
        var chevronCardParagraph = component.FindAll("p.chevron-card__description")[5];

        Assert.Equal("Show files uploaded by you.", chevronCardParagraph.InnerHtml.Trim());
        Assert.Equal("My Uploads", chevronCardLink.InnerHtml);
        Assert.Equal("/user-uploads", chevronCardLink.GetAttribute("href"));
    }
}
