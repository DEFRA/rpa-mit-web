using System.Text.Json;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Test.Models;
public class EventPropertiesTests
{
    [Fact]
    public void SerializeAndDeserialize_ReturnsExpectedValues()
    {
        var originalProperties = new EventProperties
        {
            Checkpoint = "Checkpoint1",
            Status = "Success",
            Action = new EventAction { Type = "Type1", Message = "Message1", Timestamp = DateTime.UtcNow, Data = "Data1" }
        };

        var serializedProperties = JsonSerializer.Serialize(originalProperties);
        var deserializedProperties = JsonSerializer.Deserialize<EventProperties>(serializedProperties);

        Assert.NotNull(deserializedProperties);
        Assert.Equal(originalProperties.Checkpoint, deserializedProperties!.Checkpoint);
        Assert.Equal(originalProperties.Status, deserializedProperties.Status);
        Assert.Equal(originalProperties.Action.Type, deserializedProperties.Action.Type);
        Assert.Equal(originalProperties.Action.Message, deserializedProperties.Action.Message);
    }
}
