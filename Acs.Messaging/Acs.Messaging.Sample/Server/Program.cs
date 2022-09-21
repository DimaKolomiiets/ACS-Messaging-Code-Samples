using Acs.Messaging.Sample.Server.Hubs;
using Configuration = Acs.Messaging.Sample.Server.Models.Configuration;
using Azure.Communication.PhoneNumbers;
using Azure.Communication.Sms;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(x => builder.Configuration.GetSection(nameof(Configuration.AcsConfiguration)).Get<Configuration.AcsConfiguration>());
builder.Services.AddScoped(x => new SmsClient(x.GetRequiredService<Configuration.AcsConfiguration>().ConnectionString));
builder.Services.AddScoped(x => new PhoneNumbersClient(x.GetRequiredService<Configuration.AcsConfiguration>().ConnectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapHub<NotificationHubService>("/hubs/eventlistener");
app.MapFallbackToFile("index.html");

app.Run();
