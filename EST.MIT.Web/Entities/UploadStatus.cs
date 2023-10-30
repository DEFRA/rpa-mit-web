using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;

public enum UploadStatus
{
    [Display(Name = "REQUIRED")]
    Required,

    [Display(Name = "UPLOADED")]
    Uploaded,

    [Display(Name = "UPLOADING")]
    Uploading,

    [Display(Name = "VALIDATING")]
    Validating,

    [Display(Name = "REJECTED")]
    Rejected,
}