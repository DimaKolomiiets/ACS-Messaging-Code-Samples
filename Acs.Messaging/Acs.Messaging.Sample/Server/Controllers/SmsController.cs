using Acs.Messaging.Sample.Server.Extensions;
using Acs.Messaging.Sample.Server.Hubs;
using Acs.Messaging.Sample.Server.Models;
using Acs.Messaging.Sample.Shared.Models;
using Azure.Communication.PhoneNumbers;
using Azure.Communication.Sms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Acs.Messaging.Sample.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmsController : ControllerBase
{
    private readonly SmsClient _acsSmsClient;
    private readonly PhoneNumbersClient _acsPhoneNumbersClient;
    private readonly IHubContext<NotificationHubService, INotificationHubClient> _hubContext;

    private readonly ILogger<SmsController> _logger;

    private const string ValidationCodeKey = "validationCode";

    private bool EventTypeSubscriptionValidation => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation";

    private bool EventTypeNotification => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification";

    public SmsController(
        SmsClient acsSmsClient,
        PhoneNumbersClient acsPhoneNumbersClient,
        IHubContext<NotificationHubService, INotificationHubClient> hubContext,
        ILogger<SmsController> logger)
    {
        _acsSmsClient = acsSmsClient;
        _acsPhoneNumbersClient = acsPhoneNumbersClient;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("Send")]
    public async Task<string> Send(SmsRequest smsRequest)
    {
        _logger.Info($"Sending SMS From {smsRequest.From} >> To {smsRequest.To}");

        SmsSendResult sendResult = await _acsSmsClient.SendAsync(
            from: smsRequest.From,
            to: smsRequest.To,
            message: smsRequest.MessageText,
            options: new SmsSendOptions(enableDeliveryReport: true));

        if (sendResult.Successful)
        {
            Message message = new()
            {
                Id = smsRequest.Id,
                MessageId = sendResult.MessageId,
                FromUser = new Participant { PhoneNumber = smsRequest.From },
                ToUser = new Participant { PhoneNumber = smsRequest.To },
                MessageText = smsRequest.MessageText,
                CreatedAt = DateTime.UtcNow
            };

            await _hubContext.Clients.All.MessageReceived(message).ConfigureAwait(false);

            _logger.Info($"messageId {sendResult.MessageId}");
        }
        else
        {
            _logger.Info($"HttpStatusCode {sendResult.HttpStatusCode}, ErrorMessage {sendResult.ErrorMessage}");
        }

        return sendResult.MessageId;
    }

    [HttpPost("Receive")]
    public async Task<IActionResult> Receive()
    {
        return await DecodeGridEvent(async e =>
        {
            if (e.EventType == Shared.Constants.SMSReceived)
            {
                await _hubContext.Clients.All.MessageReceived(e.ToMessage()).ConfigureAwait(false);
            }
        });
    }

    [HttpGet("PhoneNumbers")]
    public async IAsyncEnumerable<string> GetPhoneNumbers()
    {
        var phoneNumbers = _acsPhoneNumbersClient.GetPurchasedPhoneNumbersAsync();

        await foreach (var phoneNumber in phoneNumbers)
        {
            yield return phoneNumber.PhoneNumber;
        }
    }

    private async Task<IActionResult> DecodeGridEvent(Func<GridEvent, Task> action)
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);

            var jsonContent = await reader.ReadToEndAsync().ConfigureAwait(false);

            if (jsonContent == null)
            {
                _logger.Warn("jsonContent == null");
                return BadRequest();
            }

            if (EventTypeSubscriptionValidation)
            {
                var gridEvent = JsonConvert.DeserializeObject<List<GridEvent>>(jsonContent)?.FirstOrDefault();

                if (gridEvent != null)
                {
                    await action(gridEvent).ConfigureAwait(false);
                }
                else
                {
                    _logger.Warn("!clientsNotified");
                    return BadRequest();
                }

                var validationCode = gridEvent?.Data?[ValidationCodeKey] as string;

                return new JsonResult(new { validationResponse = validationCode });
            }
            else if (EventTypeNotification)
            {
                var events = JArray.Parse(jsonContent);

                foreach (var e in events)
                {
                    var details = JsonConvert.DeserializeObject<GridEvent>(e.ToString());

                    if (details != null)
                    {
                        await action(details).ConfigureAwait(false);
                    }
                }

                return Ok();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }

        return BadRequest();
    }
}