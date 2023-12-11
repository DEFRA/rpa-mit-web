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
}