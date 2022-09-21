using Acs.Messaging.Sample.Client;
using Acs.Messaging.Sample.Client.Clients;
using Acs.Messaging.Sample.Client.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<SmsClient>();
builder.Services.AddSingleton<StateContainer>();
builder.Services.AddSingleton<HubClient>();

await builder.Build().RunAsync();