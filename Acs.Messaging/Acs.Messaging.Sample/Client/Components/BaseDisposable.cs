using Acs.Messaging.Sample.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace Acs.Messaging.Sample.Client.Components;

public class BaseDisposable : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected StateContainer? State { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (State != null)
            State.OnChange += StateHasChanged;
    }

    public async ValueTask DisposeAsync()
    {
        if (State != null)
            State.OnChange -= StateHasChanged;

        await Task.CompletedTask;
    }
}