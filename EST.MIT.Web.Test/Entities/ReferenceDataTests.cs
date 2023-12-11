using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Test.Entities;

public class ReferenceDataTests
{
    [Fact]
    public void TestOrganisationProperties()
    {
        var organisation = new Organisation
        {
            code = "RPA",
            description = "organisation description",
        };

        Assert.Equal("RPA", organisation.code);
        Assert.Equal("organisation description", organisation.description);
    }

    [Fact]
    public void TestSchemeTypeProperties()
    {
        var schemeType = new SchemeType
        {
            code = "AZ",
            description = "scheme type description",
        };

        Assert.Equal("AZ", schemeType.code);
        Assert.Equal("scheme type description", schemeType.description);
    }

    [Fact]
    public void TestPaymentTypeProperties()
    {
        var paymentType = new PaymentType
        {
            code = "AZ",
            description = "payment description",
        };

        Assert.Equal("AZ", paymentType.code);
        Assert.Equal("payment description", paymentType.description);
    }
    
    [Fact]
    public void TestMainAccountProperties()
    {
        var mainAccount = new MainAccount
        {
            code = "AP",
            description = "main account description",
        };

        Assert.Equal("AP", mainAccount.code);
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
            description = "scheme code description",
        };

        Assert.Equal("AZ", schemeCode.code);
        Assert.Equal("scheme code description", schemeCode.description);
    }
}
