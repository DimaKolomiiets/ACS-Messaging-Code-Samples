using System.ComponentModel.DataAnnotations;

namespace Acs.Messaging.Sample.Shared.Models;

public class SmsRequest
{
    public string? Id { get; set; }
    [Required]
    public string From { get; set; } = default!;
    [Required]
    public string To { get; set; } = default!;
    [Required]
    public string MessageText { get; set; } = default!;
}