using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.DTOs;

[ExcludeFromCodeCoverage]
public class AppendixReferencesDTO
{
    [JsonProperty("claimReferenceNumber")]
    public string ClaimReferenceNumber { get; init; } = default!;
}