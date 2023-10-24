using Microsoft.AspNetCore.Components.Forms;

namespace Entities;

public class BulkUploadFileSummary
{
    public IBrowserFile File { get; set; } = default!;
    public string? RandomName { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public HttpResponseMessage? UploadResponse { get; set; }
    public bool IsValidFile { get; set; }
    public bool IsUploaded { get; set; } = false;

    public bool IsValid()
    {
        string[] allowedFileTypes = new string[]
        { "application/vnd.ms-excel.sheet.macroEnabled.12",
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

        if (!allowedFileTypes.Any(x => x == File.ContentType))
        {
            Errors.Add("File is not a valid type");
        }

        if (File.Size > 1e+7)
        {
            Errors.Add("File size exceeds 10MB");
        }

        if (Errors.Any())
        {
            IsValidFile = false;
            return false;
        }

        IsValidFile = true;
        return true;
    }

}