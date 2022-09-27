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

var receivedMessage = await receiver.ReceiveMessageAsync();
// DO SOMETHING THAT CAUSE ERROR OR REJECT FLOW

// send message to DLQ
await receiver.DeadLetterMessageAsync(receivedMessage, "MessageIsDead", "Message is dead");

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
