using Acs.Messaging.Sample.Shared.Models;

namespace Acs.Messaging.Sample.Server.Hubs;

public interface INotificationHubClient
{
    Task MessageReceived(Message message);
}