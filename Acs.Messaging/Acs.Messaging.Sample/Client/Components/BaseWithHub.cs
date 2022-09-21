using Acs.Messaging.Sample.Client.Clients;
using Acs.Messaging.Sample.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Acs.Messaging.Sample.Client.Components;

public class BaseWithHub : BaseDisposable
{
    [Inject]
    protected HubClient? HubClient { get; set; }

    [Inject]
    protected SmsClient? SmsClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (HubClient != null)
        {
            await HubClient.Initalize();
        }

        if (State != null)
        {
            if (State.PhoneNumbers == null || State.PhoneNumbers.Length == 0)
            {
                if (SmsClient != null)
                {
                    State.PhoneNumbers = await SmsClient.GetPhoneNumbers();
                }
            }

            if (State.User == null)
            {
                State.User = new Participant() { Name = "Me" };
            }

            if (State.PhoneNumbers != null)
            {
                foreach (var phoneNumber in State.PhoneNumbers)
                {
                    if (string.IsNullOrEmpty(State.User.PhoneNumber))
                    {
                        State.User.PhoneNumber = phoneNumber;
                    }

                    State.AddParticipant(new Participant() { PhoneNumber = phoneNumber });
                }
            }
        }
    }
}