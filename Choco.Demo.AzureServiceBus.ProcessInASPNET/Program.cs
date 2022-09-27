using Choco.Demo.AzureServiceBus;
using Choco.Demo.AzureServiceBus.ProcessInASPNET.Services.ServiceBus;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// add azure client service for dependency injection
builder.Services.AddAzureClients(azBuilder => { azBuilder.AddServiceBusClient(Config.ConnectionString); });

// add hosted services
builder.Services.AddHostedService<ServiceBusProcessorHostedService>();
builder.Services.AddHostedService<ServiceBusDLQProcessorHostedService>();

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();