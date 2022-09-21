using Acs.Messaging.Sample.Server.Extensions;
using System.Globalization;

namespace Acs.Messaging.Sample.Server.Models;

public class GridEvent<T>
    where T : class
{
    public string? Id { get; set; }
    public string? EventType { get; set; }
    public string? Subject { get; set; }
    public DateTime EventTime { get; set; }
    public T? Data { get; set; }
    public string? Topic { get; set; }
}

public class GridEvent : GridEvent<Dictionary<string, object>>
{
    public Shared.Models.Message ToMessage()
    {
        if (EventType != Shared.Constants.SMSReceived)
            throw new InvalidOperationException($"{nameof(EventType)} should be {Shared.Constants.SMSReceived}");

        object? messageId = Data?.Get("messageId", Id);
        object? fromPhoneNumber = Data?.Get("from");
        object? toPhoneNumber = Data?.Get("to", Subject?.Split('/').LastOrDefault());
        object? messageText = Data?.Get("message");
        object? receivedTimestamp = Data?.Get("receivedTimestamp");

        if (!DateTime.TryParse(receivedTimestamp as string, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime createdAt))
        {
            createdAt = EventTime;
        }

        return new Shared.Models.Message()
        {
            Id = Id,
            MessageId = messageId as string,
            FromUser = new Shared.Models.Participant() { PhoneNumber = fromPhoneNumber as string },
            ToUser = new Shared.Models.Participant() { PhoneNumber = toPhoneNumber as string },
            MessageText = messageText as string,
            CreatedAt = createdAt
        };
    }
}