using System.Text.Json;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Test.Models;
public class EventTests
{
    [Fact]
    public void SerializeAndDeserialize_ReturnsExpectedValues()
    {
        var originalEvent = new Event
        {
            Name = "TestEvent",
            Properties = new EventProperties { Checkpoint = "Checkpoint1", Status = "Success" }
        };

        var serializedEvent = JsonSerializer.Serialize(originalEvent);
        var deserializedEvent = JsonSerializer.Deserialize<Event>(serializedEvent);

        Assert.NotNull(deserializedEvent);
        Assert.Equal(originalEvent.Name, deserializedEvent!.Name);
        Assert.Equal(originalEvent.Properties.Checkpoint, deserializedEvent.Properties.Checkpoint);
        Assert.Equal(originalEvent.Properties.Status, deserializedEvent.Properties.Status);
    }
}