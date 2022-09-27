using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus.ProcessInASPNET.Dtos;
using Newtonsoft.Json;

namespace Choco.Demo.AzureServiceBus.ProcessInASPNET.Services.ServiceBus;

public class ServiceBusProcessorHostedService : IHostedService
{
    private readonly ILogger<ServiceBusProcessorHostedService> _logger;

    private readonly ServiceBusProcessor _serviceBusProcessor;

    public ServiceBusProcessorHostedService(ILogger<ServiceBusProcessorHostedService> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;

        _serviceBusProcessor = serviceBusClient.CreateProcessor(Config.QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxConcurrentCalls   = 1
        });
        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor.ProcessErrorAsync   += ErrorHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
    }

    private Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        _logger.LogInformation("Received message: {Message}", body);
        
        // deserialize
        var obj = JsonConvert.DeserializeObject<TestSendMessageDto>(body)!;
        if (obj.PleaseDead)
        {
            throw new Exception("Oh no!");
        }

        return Task.CompletedTask;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error message handling[{ErrorSource}]: {Message}", args.ErrorSource, args.Exception.Message);

        return Task.CompletedTask;
    }
}