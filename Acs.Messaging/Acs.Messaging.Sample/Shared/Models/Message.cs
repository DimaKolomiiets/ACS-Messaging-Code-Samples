using System.Text.Json.Serialization;

namespace Acs.Messaging.Sample.Shared.Models;

public class Message
{
    public string? Id { get; set; }
    public string? MessageId { get; set; }
    public Participant? FromUser { get; set; }
    public Participant? ToUser { get; set; }
    public string? MessageText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime DisplayDate { get => CreatedAt.ToLocalTime().Date; }
    [JsonIgnore]
    public MessageDirection Direction { get; set; }
}