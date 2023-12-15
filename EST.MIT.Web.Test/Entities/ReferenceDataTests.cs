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

    [Fact]
    public void TestOrganisation_MissingCode()
    {
        var organisation = new Organisation
        {
            description = "description"
        };

        Assert.Null(organisation.code);
    }

    [Fact]
    public void TestOrganisation_MissingDescription()
    {
        var organisation = new Organisation
        {
            code = "ABC"
        };

        Assert.Null(organisation.description);
    }

    [Fact]
    public void TestSchemeType_MissingCode()
    {
        var schemeType = new SchemeType
        {
            description = "description"
        };

        Assert.Null(schemeType.code);
    }

    [Fact]
    public void TestSchemeType_MissingDescription()
    {
        var schemeType = new SchemeType
        {
            code = "XYZ"
        };

        Assert.Null(schemeType.description);
    }

    [Fact]
    public void TestPaymentType_MissingCode()
    {
        var paymentType = new PaymentType
        {
            description = "description"
        };

        Assert.Null(paymentType.code);
    }

    [Fact]
    public void TestPaymentType_MissingDescription()
    {
        var paymentType = new PaymentType
        {
            code = "XYZ"
        };

        Assert.Null(paymentType.description);
    }

    [Fact]
    public void TestMainAccount_MissingCode()
    {
        var mainAccount = new MainAccount
        {
            description = "description"
        };

        Assert.Null(mainAccount.code);
    }

    [Fact]
    public void TestMainAccount_MissingDescription()
    {
        var mainAccount = new MainAccount
        {
            code = "ABC"
        };

        Assert.Null(mainAccount.description);
    }

    [Fact]
    public void TestDeliveryBody_MissingCode()
    {
        var deliveryBody = new DeliveryBody
        {
            description = "description"
        };

        Assert.Null(deliveryBody.code);
    }

    [Fact]
    public void TestDeliveryBody_MissingDescription()
    {
        var deliveryBody = new DeliveryBody
        {
            code = "123"
        };

        Assert.Null(deliveryBody.description);
    }

    [Fact]
    public void TestFundCode_MissingCode()
    {
        var fundCode = new FundCode
        {
            description = "description"
        };

        Assert.Null(fundCode.code);
    }

    [Fact]
    public void TestFundCode_MissingDescription()
    {
        var fundCode = new FundCode
        {
            code = "456"
        };

        Assert.Null(fundCode.description);
    }

    [Fact]
    public void TestMarketingYear_MissingCode()
    {
        var marketingYear = new MarketingYear
        {
            description = "description"
        };

        Assert.Null(marketingYear.code);
    }

    [Fact]
    public void TestMarketingYear_MissingDescription()
    {
        var marketingYear = new MarketingYear
        {
            code = "2023"
        };

        Assert.Null(marketingYear.description);
    }

    [Fact]
    public void TestSchemeCode_MissingCode()
    {
        var schemeCode = new SchemeCode
        {
            description = "description"
        };

        Assert.Null(schemeCode.code);
    }

    [Fact]
    public void TestSchemeCode_MissingDescription()
    {
        var schemeCode = new SchemeCode
        {
            code = "XYZ"
        };

        Assert.Null(schemeCode.description);
    }
}
