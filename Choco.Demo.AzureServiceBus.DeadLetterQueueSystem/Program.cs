using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus;

const string messageContent = "Hello, World!";

// 1.) create service bus client
var serviceBusClient = new ServiceBusClient(Config.ConnectionString);

// 2.) create sender and send message
var sender = serviceBusClient.CreateSender(Config.QueueName);
var message = new ServiceBusMessage(messageContent);

await sender.SendMessageAsync(message);

// 3.) create receiver and receive message
var receiver = serviceBusClient.CreateReceiver(Config.QueueName);

// we receive 3 times to simulate MaxDeliveryCount
// (assuming queue has config max delivery count to 3 times)
for (var _ = 0; _ < 3; _++)
{
    // received and abandon message
    // when abandon message, it will send back to queue to be available to consume by others again
    var receivedMessage = await receiver.ReceiveMessageAsync();
    await receiver.AbandonMessageAsync(receivedMessage);
}

// 4.) create DLQ receiver and receive message and its dead letter reason
var dlqReceiver = serviceBusClient.CreateReceiver(Config.QueueName, new ServiceBusReceiverOptions
{
    SubQueue = SubQueue.DeadLetter
});

var dlqReceivedMessage = await dlqReceiver.ReceiveMessageAsync();
var dlqReceivedMessageContent = dlqReceivedMessage.Body.ToString();
Console.WriteLine($"DLQ[{dlqReceivedMessage.DeadLetterReason}]: {dlqReceivedMessage.DeadLetterErrorDescription} (content='{dlqReceivedMessageContent}')");

// complete receiving (consumed)
await dlqReceiver.CompleteMessageAsync(dlqReceivedMessage);
