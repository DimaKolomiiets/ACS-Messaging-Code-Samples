namespace Acs.Messaging.Sample.Shared.Models;

public class Messages : Dictionary<string, Message>
{
    public Dictionary<DateTime, List<Message>>? GetByPhone(string phoneNumber)
    {
        return Values
            .Where(i => i.FromUser?.PhoneNumber == phoneNumber || i.ToUser?.PhoneNumber == phoneNumber)
            .GroupBy(i => i.DisplayDate)
            .Select(g => new { Date = g.Key, Items = g.OrderBy(j => j.CreatedAt).ToList() })
            .OrderBy(i => i.Date)
            .ToDictionary(i => i.Date, i => i.Items);
    }

    public DateTime? GetLastMessageDateFromPhone(string? phoneNumber)
    {
        DateTime result = Values
            .Where(i => i.FromUser?.PhoneNumber == phoneNumber)
            .Select(i => i.CreatedAt.ToLocalTime())
            .DefaultIfEmpty()
            .Max();

        return result == DateTime.MinValue ? null : result;
    }
}