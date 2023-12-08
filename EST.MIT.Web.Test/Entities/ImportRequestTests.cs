using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Test.Entities;

public class ImportRequestTests
{
    [Fact]
    public void TestExcelLineProperties()
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
            BlobFileName = "BlobFileName",
            BlobFolder = "BlobFolder"
        };

        Assert.Equal(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"), importRequest.ImportRequestId);
        Assert.Equal("test.xlsx", importRequest.FileName);
        Assert.Equal(1024, importRequest.FileSize);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", importRequest.FileType);
        Assert.NotNull(importRequest.Timestamp);
        Assert.Equal("AR", importRequest.PaymentType);
        Assert.Equal("RDT", importRequest.Organisation);
        Assert.Equal("CP", importRequest.SchemeType);
        Assert.Equal("First Payment", importRequest.AccountType);
        Assert.Equal("test@example.com", importRequest.CreatedBy);
        Assert.Equal(UploadStatus.Upload_success, importRequest.Status);
        Assert.Equal("test.xlsx", importRequest.FileName);
        Assert.Equal("BlobFileName", importRequest.BlobFileName);
        Assert.Equal("BlobFolder", importRequest.BlobFolder);
    }
}