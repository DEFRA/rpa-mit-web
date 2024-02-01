using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using System.Net;
using EST.MIT.Web.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using EST.MIT.Web.Pages.bulk.BulkUpload;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared;

namespace EST.MIT.Web.Tests.Pages;

public class BulkUploadPageTests : TestContext
{
    private IConfiguration _configuration;
    private readonly Invoice _invoice;

    public BulkUploadPageTests()
    {
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("connection_string");
        configMock.Setup(x => x.GetSection(It.Is<string>(s => s == "ConnectionStrings:PrimaryConnection"))).Returns(configSectionMock.Object);
        _configuration = configMock.Object;

        _invoice = new()
        {
            AccountType = "AR",
            PaymentType = "DOM",
            Organisation = "RPA",
            SchemeType = "BPS"
        };
        var mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        var mockAzureBlobService = new Mock<IAzureBlobService>();
        var mockInvoiceRepository = new Mock<IInvoiceRepository>();

        mockAzureBlobService.Setup(x => x.blobServiceClient).Returns(mockBlobServiceClient.Object);
        mockBlobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

        var mockEventQueueService = new Mock<IEventQueueService>();
        var mockImporterQueueService = new Mock<IImporterQueueService>();
        var mockQueueServiceClient = new Mock<QueueServiceClient>();
        var mockQueueClient = new Mock<QueueClient>();

        mockQueueServiceClient.Setup(x => x.GetQueueClient(It.IsAny<string>())).Returns(mockQueueClient.Object);
        mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        Services.AddSingleton<IConfiguration>(_configuration);
        Services.AddSingleton<IUploadService, UploadService>();
        Services.AddSingleton<IEventQueueService>(mockEventQueueService.Object);
        Services.AddSingleton<IAzureBlobService>(mockAzureBlobService.Object);
        Services.AddSingleton<IInvoiceRepository>(mockInvoiceRepository.Object);
        Services.AddSingleton<IInvoiceStateContainer>(mockInvoiceStateContainer.Object);
        Services.AddSingleton<IImporterQueueService>(mockImporterQueueService.Object);
    }

    [Fact]
    public void Change_Validates_Correct_File()
    {
        InputFileContent fileToUpload = InputFileContent.CreateFromText("test file", "TestFile.xlsm", null, "application/vnd.ms-excel.sheet.macroEnabled.12");

        IRenderedComponent<BulkUpload> component = RenderComponent<BulkUpload>();
        IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

        inputFile.UploadFiles(fileToUpload);

        component.Instance.error.Should().BeFalse();
        component.Instance.errorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Change_Validates_Wrong_Filetype()
    {
        InputFileContent fileToUpload = InputFileContent.CreateFromText("test file", "TestCsvFile.txt");

        IRenderedComponent<BulkUpload> component = RenderComponent<BulkUpload>();
        IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

        inputFile.UploadFiles(fileToUpload);

        component.Instance.error.Should().BeTrue();
        component.Instance.errorMessage.Should().Be("File is not a valid type");
    }

    [Fact]
    public void Change_Validates_Wrong_Filesize()
    {
        InputFileContent fileToUpload = InputFileContent.CreateFromBinary(new byte[(long)1e+8], "TestFile.xlsm", null, "application/vnd.ms-excel.sheet.macroEnabled.12");

        IRenderedComponent<BulkUpload> component = RenderComponent<BulkUpload>();
        IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

        inputFile.UploadFiles(fileToUpload);

        component.Instance.error.Should().BeTrue();
        component.Instance.errorMessage.Should().Be("File size exceeds 10MB");
    }

    [Fact]
    public void Validated_File_Added_To_Blob()
    {
        InputFileContent fileToUpload = InputFileContent.CreateFromText("test file", "TestFile.xlsm", null, "application/vnd.ms-excel.sheet.macroEnabled.12");

        IRenderedComponent<BulkUpload> component = RenderComponent<BulkUpload>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

        inputFile.UploadFiles(fileToUpload);

        var submit = component.FindAll("button[type=submit]")[0];
        submit.Click();

        component.WaitForState(() => component.Instance.fileToLoadSummary.IsUploaded != false);

        component.Instance.fileToLoadSummary.IsValidFile.Should().BeTrue();
        component.Instance.fileToLoadSummary.IsUploaded.Should().BeTrue();
    }

    [Fact]
    public void Validated_File_Not_Added_To_Blob()
    {
        Mock<IUploadService> uploadServiceMock = new Mock<IUploadService>();
        uploadServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IBrowserFile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest }));

        Services.AddSingleton<IConfiguration>(_configuration);
        Services.AddSingleton<IUploadService>(uploadServiceMock.Object);
        Services.AddSingleton<Mock<IUploadService>>(uploadServiceMock);
        Services.AddSingleton<IAzureBlobService, AzureBlobService>();
        Services.AddSingleton<IEventQueueService, EventQueueService>();

        IRenderedComponent<BulkUpload> component = RenderComponent<BulkUpload>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

        component.Instance.fileToLoadSummary.IsValidFile = true;

        var submit = component.FindAll("button[type=submit]")[0];
        submit.Click();

        component.Instance.fileToLoadSummary.IsUploaded.Should().BeFalse();
        component.Instance.error.Should().BeTrue();
    }
}