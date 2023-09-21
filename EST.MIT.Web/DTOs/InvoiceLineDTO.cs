using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.DTOs;

[ExcludeFromCodeCoverage]
public class InvoiceLineDTO
{
    [JsonProperty("value")]
    public decimal Value { get; set; }

    [JsonProperty("fundCode")]
    public string FundCode { get; init; } = default!;

    [JsonProperty("mainAccount")]
    public string MainAccount { get; init; } = default!;

    [JsonProperty("schemeCode")]
    public string SchemeCode { get; init; } = default!;

    [JsonProperty("marketingYear")]
    public int MarketingYear { get; init; }

    [JsonProperty("deliveryBody")]
    public string DeliveryBody { get; init; } = default!;

    [JsonProperty("description")]
    public string Description { get; init; } = default!;
}