using System.Diagnostics.CodeAnalysis;
namespace EST.MIT.Web.Entities;

[ExcludeFromCodeCoverage]
public class Organisation
{
    public string code { get; set; }
    public string description { get; set; }
}

[ExcludeFromCodeCoverage]
public class PaymentScheme
{
    public string code { get; set; }
    public string description { get; set; }
}