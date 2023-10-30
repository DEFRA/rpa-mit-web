using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.Uploads.UserUploads;
using EST.MIT.Web.Shared.Components.UserUploadsCard;
using EST.MIT.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EST.MIT.Web.Test.Pages.Uploads;

public class UserUploadsTests : TestContext
{
    private Mock<IUploadAPI> _mockApiService;
    private List<ImportRequest> _mockUploads;

    public UserUploadsTests()
    {
        _mockApiService = new Mock<IUploadAPI>();
        _mockUploads = new List<ImportRequest>
        {
            new ImportRequest
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
            }
        };

        _mockApiService.Setup(api => api.GetUploadsAsync()).ReturnsAsync(_mockUploads);

        Services.AddSingleton<IUploadAPI>(_mockApiService.Object);
    }

    [Fact]
    public void Page_Calls_API_On_Initialized()
    {
        var component = RenderComponent<UserUploads>();

        _mockApiService.Verify(api => api.GetUploadsAsync(), Times.Once);
    }

    [Fact]
    public void Page_Renders_UserUploadsCard_With_Correct_Data()
    {
        var component = RenderComponent<UserUploads>();
        var userUploadsCard = component.FindComponent<UserUploadsCard>();

        Assert.NotNull(userUploadsCard);
        Assert.Equal(_mockUploads, userUploadsCard.Instance.importRequests);

        var cellTexts = userUploadsCard.FindAll(".govuk-table__cell").Select(cell => cell.TextContent).ToArray();
        Assert.Equal(_mockUploads.First().Timestamp?.ToString("dd/MM/yyyy"), cellTexts[0]);
        Assert.Equal("Uploaded", cellTexts[1]);
        Assert.Equal("First Payment", cellTexts[2]);
        Assert.Equal("RDT", cellTexts[3]);
        Assert.Equal("CP", cellTexts[4]);
        Assert.Equal("AR", cellTexts[5]);
    }

    [Fact]
    public void Correct_Number_Of_Rows_Rendered()
    {
        var importRequests = new List<ImportRequest>
        {
            new ImportRequest(),
            new ImportRequest()
        };

        var component = RenderComponent<UserUploadsCard>(parameters => parameters.Add(p => p.importRequests, importRequests));

        var rows = component.FindAll(".govuk-table__row");
        rows.Count.Should().Be(importRequests.Count + 1);
    }
}