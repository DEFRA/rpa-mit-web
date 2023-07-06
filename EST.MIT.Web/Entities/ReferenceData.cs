using System.Diagnostics.CodeAnalysis;
namespace Entities;

[ExcludeFromCodeCoverageAttribute]
public class Organisation
{
    public string code { get; set; }
    public string description { get; set; }
}

[ExcludeFromCodeCoverageAttribute]
public class PaymentScheme
{
    public string code { get; set; }
    public string description { get; set; }
}