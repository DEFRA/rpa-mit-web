using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public enum UploadStatus
{
    [Display(Name = "Upload success")]
    Upload_success,

    [Display(Name = "Upload failed")]
    Upload_failed,
}