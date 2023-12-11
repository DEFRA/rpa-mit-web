using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Test.Entities;

public class ReferenceDataTests
{
    [Fact]
    public void TestMainAccountProperties()
    {
        var mainAccount = new MainAccount
        {
            code = "AZ",
            description = "main account description",
        };

        Assert.Equal("AZ", mainAccount.code);
        Assert.Equal("main account description", mainAccount.description);
    }

    [Fact]
    public void TestDeliveryBodyProperties()
    {
        var deliveryBody = new DeliveryBody
        {
            code = "AX",
            description = "delivery body description",
        };

        Assert.Equal("AX", deliveryBody.code);
        Assert.Equal("delivery body description", deliveryBody.description);
    }

    [Fact]
    public void TestFundCodeProperties()
    {
        var fundCode = new FundCode
        {
            code = "RPA",
            description = "fund code description",
        };

        Assert.Equal("RPA", fundCode.code);
        Assert.Equal("fund code description", fundCode.description);
    }

    [Fact]
    public void TestMarketingYearProperties()
    {
        var marketingYear = new MarketingYear
        {
            code = "2023",
            description = "marketing year description",
        };

        Assert.Equal("2023", marketingYear.code);
        Assert.Equal("marketing year description", marketingYear.description);
    }

    [Fact]
    public void TestSchemeCodeProperties()
    {
        var schemeCode = new SchemeCode
        {
            code = "AZ",
            description = "marketing year description",
        };

        Assert.Equal("AZ", schemeCode.code);
        Assert.Equal("marketing year description", schemeCode.description);
    }
}
