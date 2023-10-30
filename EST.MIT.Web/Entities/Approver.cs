using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Web.Entities;
public class ApproverSelect
{
    private string _approverEmail;

    [Required(ErrorMessage = "Please enter an email address")]
    [EmailFormatCheck(ErrorMessage = "Invalid email format")]
    public string ApproverEmail
    {
        get => _approverEmail;
        set => _approverEmail = value.ToLower();
    }
}

public class DomainCheck : RegularExpressionAttribute
{
    private const string domainRegex = @"@(rpa|defra)\.gov\.uk$";
    public DomainCheck() : base(domainRegex) { }
}

public class EmailFormatCheck : RegularExpressionAttribute
{
    private const string formatRegex = @"^[a-z]+\.[a-z]+@[a-z]+\.gov\.uk$";
    public EmailFormatCheck() : base(formatRegex) { }
}
