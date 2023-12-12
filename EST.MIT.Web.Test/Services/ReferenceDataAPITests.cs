using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;

namespace Repositories.Tests;

public class ReferenceDataAPITests
{
    private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepository;
    public ReferenceDataAPITests()
    {
        _mockReferenceDataRepository = new Mock<IReferenceDataRepository>();
    }
    //Organisation
    [Fact]
    public async Task GetOrganisationsAsync_Returns_List_Organisation()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<Organisation>()
                {
                    new Organisation()
                    {
                        code = "RPA",
                        description = "A nice place to work"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<Organisation>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<Organisation>()
        {
            new Organisation()
            {
                code = "RPA",
                description = "A nice place to work"
            }
        });

    }

    [Fact]
    public async Task GetOrganisationsAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetOrganisationsAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetOrganisationsAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetOrganisationsAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetOrganisationsAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetOrganisationsAsync("AP");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    //Scheme type
    [Fact]
    public async Task GetSchemesAsync_Returns_List_Organisation()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<SchemeType>()
                {
                    new SchemeType()
                    {
                        code = "RPA",
                        description = "A nice place to work"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<SchemeType>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<SchemeType>()
        {
            new SchemeType()
            {
                code = "RPA",
                description = "A nice place to work"
            }
        });

    }

    [Fact]
    public async Task GetSchemesAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetSchemesAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetSchemesAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetSchemesAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetSchemesAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemeTypesListAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemeTypesAsync("AP", "RPA");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    // Payment type

    [Fact]
    public async Task GetPaymentTypeAsync_Returns_List_PaymentTypes()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<PaymentType>()
                {
                    new PaymentType()
                    {
                        code = "RPA",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<PaymentType>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<PaymentType>()
        {
            new PaymentType()
            {
                code = "RPA",
                description = "Some description"
            }
        });
    }

    [Fact]
    public async Task GetPaymentTypeAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetPaymentTypeAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetPaymentTypeAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetPaymentTypeAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetPaymentTypeAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetPaymentTypesAsync("AP", "RPA", "AZ");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    // Main account
    [Fact]
    public async Task GetMainAccountAsync_Returns_List_MainAccounts()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<MainAccount>()
                {
                    new MainAccount()
                    {
                        code = "AX",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<MainAccount>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<MainAccount>()
        {
            new MainAccount()
            {
                code = "AX",
                description = "Some description"
            }
        });

    }

    [Fact]
    public async Task GetMainAccountAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMainAccountAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AB");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetMainAccountAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMainAccountAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetMainAccountAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetAccountsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetAccountsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    //Delivery body
    [Fact]
    public async Task GetDeliveryBodyAsync_Returns_List_DeliveryBodies()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<DeliveryBody>()
                {
                    new DeliveryBody()
                    {
                        code = "BA",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<DeliveryBody>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<DeliveryBody>()
        {
            new DeliveryBody()
            {
                code = "BA",
                description = "Some description"
            }
        });
    }

    [Fact]
    public async Task GetDeliveryBodyAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetDeliveryBodyAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AB");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetDeliveryBodyAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetDeliveryBodyAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetDeliveryBodyAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetDeliveryBodiesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetDeliveryBodiesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    [Fact]
    public async Task GetFundCodeAsync_Returns_List_FundCodes()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<FundCode>()
                {
                    new FundCode()
                    {
                        code = "BA",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<FundCode>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<FundCode>()
        {
            new FundCode()
            {
                code = "BA",
                description = "Some description"
            }
        });
    }

    [Fact]
    public async Task GetFundCodeAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetFundCodeAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AB");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetFundCodeAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetFundCodeAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetFundCodeAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetFundsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetFundsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    // Marketing year
    [Fact]
    public async Task GetMarketingYearAsync_Returns_List_MarketingYears()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<MarketingYear>()
                {
                    new MarketingYear()
                    {
                        code = "2023",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<MarketingYear>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<MarketingYear>()
        {
            new MarketingYear()
            {
                code = "2023",
                description = "Some description"
            }
        });
    }

    [Fact]
    public async Task GetMarketingYearAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMarketingYearAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AB");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetMarketingYearAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMarketingYearAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetMarketingYearAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetMarketingYearsListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetMarketingYearsAsync("AP", "RPA", "AZ", "AD");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    // Scheme code

    [Fact]
    public async Task GetSchemeCodeAsync_Returns_List_MarketingYears()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<SchemeCode>()
                {
                    new SchemeCode()
                    {
                        code = "AX",
                        description = "Some description"
                    }
                }))
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<SchemeCode>>();
        response.Data.Should().HaveCount(1);
        response.Data.Should().BeEquivalentTo(new List<SchemeCode>()
        {
            new SchemeCode()
            {
                code = "AX",
                description = "Some description"
            }
        });
    }

    [Fact]
    public async Task GetSchemeCodeAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetSchemeCodeAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AB");

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public async Task GetSchemeCodeAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetSchemeCodeAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public async Task GetSchemeCodeAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataAPI(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataAPI>>());

        var response = await service.GetSchemesAsync("AP", "RPA", "AZ", "AB");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }
}