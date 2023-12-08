using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Test.Entities;

public class UploadStatusTests
{
    [Theory]
    [InlineData(UploadStatus.Upload_successful, "Upload successful")]
    [InlineData(UploadStatus.Upload_failed, "Upload failed")]
    [InlineData(UploadStatus.Upload_validated, "Upload validated")]
    [InlineData(UploadStatus.Uploaded, "Uploaded")]
    public void DisplayAttributeIsCorrect(UploadStatus status, string expectedName)
    {
        var memberInfo = status.GetType().GetMember(status.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

        var displayAttribute = (DisplayAttribute)attributes[0];

        Assert.Equal(expectedName, displayAttribute.Name);
    }
}
