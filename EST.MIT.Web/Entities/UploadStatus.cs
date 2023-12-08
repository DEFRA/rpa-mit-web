using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EST.MIT.Web.Entities;

public enum UploadStatus
{
    [Display(Name = "Upload successful")]
    Upload_successful,

    [Display(Name = "Upload failed")]
    Upload_failed,

    [Display(Name = "Upload validated")]
    Upload_validated,

    [Display(Name = "Uploaded")]
    Uploaded
}