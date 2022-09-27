using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus;

const string messageContent = "Hello, World!";

long deferredMessageSequence;

// 1.) create service bus client
var serviceBusClient = new ServiceBusClient(Config.ConnectionString);

// 2.) create sender and send message
var sender = serviceBusClient.CreateSender(Config.QueueName);
var message = new ServiceBusMessage(messageContent);

await sender.SendMessageAsync(message);

// 3.) create receiver and receive message
var receiver = serviceBusClient.CreateReceiver(Config.QueueName);

var receivedMessage = await receiver.ReceiveMessageAsync();
// keep its sequence number for future receive
deferredMessageSequence = receivedMessage.SequenceNumber;

// defer message
await receiver.DeferMessageAsync(receivedMessage);
Console.WriteLine($"Message sequence number={deferredMessageSequence} deferred");
// when we defer message, the message is unable to receive normally
// we need to use its deferred message sequence number to retrieve the message from queue

// DO SOMETHING FOR A VERY LONG TIME...

// 4.) receive deferred message
var receivedDeferredMessage = await receiver.ReceiveDeferredMessageAsync(deferredMessageSequence);
var receivedDeferredMessageContent = receivedDeferredMessage.Body.ToString();
Console.WriteLine(receivedDeferredMessageContent);

// complete receiving (consumed)
await receiver.CompleteMessageAsync(receivedDeferredMessage);
