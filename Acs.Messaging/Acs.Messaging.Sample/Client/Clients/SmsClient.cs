using Acs.Messaging.Sample.Client.Shared;
using Acs.Messaging.Sample.Shared.Models;
using System.Net.Http.Json;

namespace Acs.Messaging.Sample.Client.Clients;

public class SmsClient
{
    private readonly HttpClient _httpClient;
    private readonly StateContainer _state;

    public SmsClient(HttpClient httpClient, StateContainer state)
    {
        _httpClient = httpClient;
        _state = state;
    }

    public async Task Send(SmsRequest smsRequest)
    {
        await _httpClient.PostAsJsonAsync<SmsRequest>("api/Sms/Send", smsRequest);
    }

    public async Task SendSms(string? messageText)
    {
        if (string.IsNullOrEmpty(messageText))
            throw new ArgumentNullException(nameof(messageText));

        if (string.IsNullOrEmpty(_state?.User?.PhoneNumber))
            throw new ArgumentNullException("From Phone Number");

        if (string.IsNullOrEmpty(_state?.CurrentContact?.PhoneNumber))
            throw new ArgumentNullException("To Phone Number");

        SmsRequest smsRequest = new()
        {
            Id = Guid.NewGuid().ToString(),
            From = _state.User.PhoneNumber,
            To = _state.CurrentContact.PhoneNumber,
            MessageText = messageText
        };

        await Send(smsRequest);
    }

    public async Task<string[]?> GetPhoneNumbers()
    {
        return await _httpClient.GetFromJsonAsync<string[]>("api/Sms/PhoneNumbers");
    }
}