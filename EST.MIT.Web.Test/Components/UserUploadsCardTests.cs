using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared.Components.UserUploadsCard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace EST.MIT.Web.Test.Components;

public class UserUploadsCardTests : TestContext
{
    private readonly Mock<IUploadAPI> _mockUploadAPI;
    private readonly Mock<IJSRuntime> _mockJSRuntime;

    public UserUploadsCardTests()
    {
        _mockUploadAPI = new Mock<IUploadAPI>();
        _mockJSRuntime = new Mock<IJSRuntime>();
        Services.AddSingleton(_mockUploadAPI.Object);
        Services.AddSingleton(_mockJSRuntime.Object);
    }

    [Fact]
    public void Specific_Data_Rendered_Correctly()
    {
        var importRequest = new ImportRequest
        {
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment",
            CreatedBy = "test@example.com",
            Status = UploadStatus.Upload_success,
            BlobFileName = "testblob",
            BlobFolder = "archive",
            Email = "email@defra.gov.uk"
        };

        var importRequests = new List<ImportRequest> { importRequest };

        var component = RenderComponent<UserUploadsCard>(parameters => parameters.Add(p => p.importRequests, importRequests));

        var cellTexts = component.FindAll(".govuk-table__cell").Select(cell => cell.TextContent).ToArray();
        cellTexts[0].Should().Be(importRequest.Timestamp.Value.ToString("dd/MM/yyyy HH:mm:ss"));
        cellTexts[1].Should().Be("First Payment");
        cellTexts[2].Should().Be("RDT");
        cellTexts[3].Should().Be("CP");
        cellTexts[4].Should().Be("AR");
        cellTexts[5].Should().Be("Upload_success");

        var downloadLinkCell = component.FindAll(".govuk-table__cell")[6];
        var downloadLink = downloadLinkCell.QuerySelector("a");
        downloadLink.TextContent.Should().Contain(importRequest.FileName);
    }

    [Fact]
    public void Status_Tag_Green_On_Upload_Success()
    {
        var importRequest = new ImportRequest
        {
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment",
            CreatedBy = "test@example.com",
            Status = UploadStatus.Upload_success,
            BlobFileName = "testblob",
            BlobFolder = "archive",
            Email = "email@defra.gov.uk"
        };

        var importRequests = new List<ImportRequest> { importRequest };

        var component = RenderComponent<UserUploadsCard>(parameters => parameters.Add(p => p.importRequests, importRequests));

        var statusCell = component.FindAll(".govuk-table__cell")[5];
        var statusTag = statusCell.QuerySelector("strong");
        statusTag.ClassName.Should().Contain("govuk-tag govuk-tag--green");
    }

    [Fact]
    public void Status_Tag_Red_On_Upload_Failed()
    {
        var importRequest = new ImportRequest
        {
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment",
            CreatedBy = "test@example.com",
            Status = UploadStatus.Upload_failed,
            BlobFileName = "testblob",
            BlobFolder = "archive",
            Email = "email@defra.gov.uk"
        };

        var importRequests = new List<ImportRequest> { importRequest };

        var component = RenderComponent<UserUploadsCard>(parameters => parameters.Add(p => p.importRequests, importRequests));

        var statusCell = component.FindAll(".govuk-table__cell")[5];
        var statusTag = statusCell.QuerySelector("strong");
        statusTag.ClassName.Should().Contain("govuk-tag govuk-tag--red");
    }

    [Fact]
    public void Nothing_Displayed_When_ImportRequests_Is_Null()
    {
        var component = RenderComponent<UserUploadsCard>();
        component.Markup.Should().Contain("<h4 class=\"govuk-heading-s\">Nothing to Display</h4>");
    }

    [Fact]
    public void GetFileStream_Returns_Non_Null_Stream()
    {
        var component = RenderComponent<UserUploadsCard>();

        var fileStream = component.Instance.GetFileStream();

        fileStream.Should().NotBeNull();
    }
}
