using System.Text.Json.Serialization;

namespace EST.MIT.Web.Models;

public class EventProperties
{
    [JsonPropertyName("checkpoint")]
    public string Checkpoint { get; init; } = default!;
    [JsonPropertyName("status")]
    public string Status { get; init; } = default!;
    [JsonPropertyName("action")]
    public EventAction Action { get; init; } = default!;
}
