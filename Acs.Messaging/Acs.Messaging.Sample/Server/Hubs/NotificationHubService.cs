using Microsoft.AspNetCore.SignalR;

namespace Acs.Messaging.Sample.Server.Hubs;

public class NotificationHubService : Hub<INotificationHubClient>
{
}