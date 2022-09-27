using Azure.Messaging.ServiceBus;

namespace Choco.Demo.AzureServiceBus.ProcessInASPNET.Services.ServiceBus;

public class ServiceBusDLQProcessorHostedService : IHostedService
{
    private readonly ILogger<ServiceBusDLQProcessorHostedService> _logger;

    private readonly ServiceBusProcessor _serviceBusProcessor;

    public ServiceBusDLQProcessorHostedService(ILogger<ServiceBusDLQProcessorHostedService> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;

        _serviceBusProcessor = serviceBusClient.CreateProcessor(Config.QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxConcurrentCalls   = 1,
            SubQueue             = SubQueue.DeadLetter
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
        _logger.LogInformation("Received dead letter[{DlqReason}]: {DlqDesc} (content='{Message}')", 
            args.Message.DeadLetterReason, args.Message.DeadLetterErrorDescription, body);

        return Task.CompletedTask;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error message handling[{ErrorSource}]: {Message}", args.ErrorSource, args.Exception.Message);

        return Task.CompletedTask;
    }
}