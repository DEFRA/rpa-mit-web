using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EST.MIT.Web.Shared.Components.UserUploadsCard;

public partial class UserUploadsCard : ComponentBase
{
    [Inject] private IUploadAPI _API { get; set; }

    [Parameter] public IEnumerable<ImportRequest> importRequests { get; set; }

    static string FormatTimestamp(DateTimeOffset? timestamp)
    {
        if (timestamp == null)
        {
            return "N/A";
        }
        return timestamp.Value.ToString("dd/MM/yyyy");
    }

    private Stream GetFileStream()
    {
        var binaryData = new byte[50 * 1024];
        var fileStream = new MemoryStream(binaryData);
        return fileStream;
    }

    private async Task DownloadFile(string fileName)
    {
        var fileBytes = await _API.GetFileByFileNameAsync(fileName);
        if (fileBytes == null || fileBytes.Length == 0)
        {
            return;
        }

        using var fileStream = new MemoryStream(fileBytes);
        using var streamRef = new DotNetStreamReference(stream: fileStream);

        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
}