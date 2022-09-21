namespace Acs.Messaging.Sample.Shared.Models;

public class Participant
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageUri { get; set; }
    public string? PhoneDisplay => Utils.FormatPhoneNumber(PhoneNumber);
}