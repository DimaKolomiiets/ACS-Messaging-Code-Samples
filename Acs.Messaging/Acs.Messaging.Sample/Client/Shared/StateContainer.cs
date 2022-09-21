using Acs.Messaging.Sample.Shared.Models;

namespace Acs.Messaging.Sample.Client.Shared;

public class StateContainer
{
    private Participant? user;
    public Participant? User
    {
        get => user;
        set
        {
            user = value;
            NotifyChanged();
        }
    }

    private Participant? currentContact;
    public Participant? CurrentContact
    {
        get => currentContact;
        set
        {
            currentContact = value;
            NotifyChanged();
        }
    }

    private string[]? phoneNumbers;
    public string[]? PhoneNumbers
    {
        get => phoneNumbers;
        set
        {
            phoneNumbers = value;
            NotifyChanged();
        }
    }

    private Dictionary<string, Participant> participants = new();
    public Dictionary<string, Participant> Participants
    {
        get => participants;
        set
        {
            participants = value;
            NotifyChanged();
        }
    }

    private Messages messages = new();
    public Messages Messages
    {
        get => messages;
        set
        {
            messages = value;
            NotifyChanged();
        }
    }

    public event Action? OnChange;

    public void NotifyChanged() => OnChange?.Invoke();

    private readonly string[] avatars = new[] {
        "/img/man.06.png",
        "/img/woman.04.png",
        "/img/man.09.png",
        "/img/man.08.png",
        "/img/woman.03.png",
        "/img/man.00.png",
        "/img/woman.00.png",
        "/img/woman.01.png",
        "/img/man.01.png",
        "/img/man.02.png",
        "/img/woman.02.png",
        "/img/man.03.png",
        "/img/man.04.png",
        "/img/man.05.png",
        "/img/woman.05.png",
        "/img/man.07.png",
        "/img/woman.06.png",
        "/img/woman.07.png"
    };

    public Participant? AddParticipant(Participant participant)
    {
        if (participant == null)
            return null;

        if (string.IsNullOrEmpty(participant.PhoneNumber))
            return null;

        if (participant.PhoneNumber == User?.PhoneNumber)
            return null;

        if (string.IsNullOrEmpty(participant.Id))
        {
            participant.Id = Guid.NewGuid().ToString();
        }

        if (string.IsNullOrEmpty(participant.ImageUri))
            participant.ImageUri = avatars[participants.Count % avatars.Length];

        if (!participants.TryAdd(participant.PhoneNumber, participant))
        {
            var currentParticipant = participants[participant.PhoneNumber];

            if (string.IsNullOrEmpty(currentParticipant.Name))
                currentParticipant.Name = participant.Name;

            if (string.IsNullOrEmpty(currentParticipant.ImageUri))
                currentParticipant.ImageUri = participant.ImageUri;

            participants[participant.PhoneNumber] = participant = currentParticipant;
        }

        if (CurrentContact == null)
            CurrentContact = participant;

        NotifyChanged();

        return participant;
    }

    public Participant? GetParticipantByPhone(string phoneNumber)
    {
        if (participants.ContainsKey(phoneNumber))
            return participants[phoneNumber];

        return null;
    }
    
    public string? GetParticipantIdByPhone(string phoneNumber)
    {
        if (participants.ContainsKey(phoneNumber))
            return participants[phoneNumber].Id;

        return null;
    }

    public void AddMessage(Message message)
    {
        EnsureDirection(message);

        if (message.Direction == MessageDirection.NA)
            return;

        if (string.IsNullOrEmpty(message.Id))
        {
            message.Id = Guid.NewGuid().ToString();
        }

        if (!messages.ContainsKey(message.Id))
            messages.Add(message.Id, message);

        NotifyChanged();
    }

    private void EnsureDirection(Message message)
    {
        if (message == null)
            return;

        if (message.Direction != MessageDirection.NA)
            return;

        if (message.FromUser?.PhoneNumber == user?.PhoneNumber)
        {
            message.Direction = MessageDirection.Outbound;
        }

        if (message.ToUser?.PhoneNumber == user?.PhoneNumber)
        {
            message.Direction = MessageDirection.Inbound;
        }
    }
}