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
            Status = UploadStatus.Uploaded,
            BlobPath = "https://defrastorageaccount.blob.core.windows.net/invoices/import/test.xlsx"
        };

        var importRequests = new List<ImportRequest> { importRequest };

        var component = RenderComponent<UserUploadsCard>(parameters => parameters.Add(p => p.importRequests, importRequests));

        var cellTexts = component.FindAll(".govuk-table__cell").Select(cell => cell.TextContent).ToArray();
        cellTexts[0].Should().Be(importRequest.Timestamp.Value.ToString("dd/MM/yyyy"));
        cellTexts[1].Should().Be("Uploaded");
        cellTexts[2].Should().Be("First Payment");
        cellTexts[3].Should().Be("RDT");
        cellTexts[4].Should().Be("CP");
        cellTexts[5].Should().Be("AR");
        cellTexts[6] = component.FindAll(".govuk-table__cell")[6].TextContent.Trim();
    }

    [Fact]
    public void Nothing_Displayed_When_ImportRequests_Is_Null()
    {
        var component = RenderComponent<UserUploadsCard>();
        component.Markup.Should().Contain("<h4 class=\"govuk-heading-s\">Nothing to Display</h4>");
    }
}
