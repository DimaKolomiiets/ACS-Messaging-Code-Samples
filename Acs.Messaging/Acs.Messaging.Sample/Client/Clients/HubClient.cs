using Acs.Messaging.Sample.Client.Shared;
using Acs.Messaging.Sample.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Acs.Messaging.Sample.Client.Clients;

public class HubClient : IAsyncDisposable
{
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    private readonly NavigationManager? _navigationManager;
    private readonly StateContainer? _state;

    private HubConnection? _hubConnection;

    public HubClient(NavigationManager navigationManager, StateContainer state)
    {
        _navigationManager = navigationManager;
        _state = state;
    }

    public async Task Initalize()
    {
        if (_hubConnection == null)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager?.ToAbsoluteUri("/hubs/eventlistener")!)
                .Build();

            _hubConnection.Closed += error =>
            {
                _navigationManager?.NavigateTo(_navigationManager.Uri, forceLoad: true);
                return Task.CompletedTask;
            };

            _hubConnection.On<Message>("MessageReceived", message =>
            {
                Console.WriteLine($"ReceiveMessage: Id {message.Id}, MessageId {message.MessageId}, from {message.FromUser?.PhoneNumber}, to {message.ToUser?.PhoneNumber}");

                if (message.FromUser?.PhoneNumber == _state?.User?.PhoneNumber ||
                    message.ToUser?.PhoneNumber == _state?.User?.PhoneNumber)
                {
                    _state?.AddParticipant(message.FromUser!);
                    _state?.AddParticipant(message.ToUser!);
                    _state?.AddMessage(message);
                }
            });
        }

        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}