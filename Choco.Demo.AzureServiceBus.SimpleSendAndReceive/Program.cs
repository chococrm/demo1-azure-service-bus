using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus;

const string messageContent = "Hello, World!";
object context = new
{
    A = "hello, World"
};

// 1.) create service bus client
var serviceBusClient = new ServiceBusClient(Config.ConnectionString);

// 2.) create sender and send message
var sender = serviceBusClient.CreateSender(Config.QueueName);
var message = new ServiceBusMessage(messageContent);

await sender.SendMessageAsync(message);

// 3.) create receiver and receive message
// PeekLock is a default behavior when receive message
var receiver = serviceBusClient.CreateReceiver(Config.QueueName);

var receivedMessage = await receiver.ReceiveMessageAsync();
var receivedMessageContent = receivedMessage.Body.ToString();
Console.WriteLine(receivedMessageContent);

// complete receiving (consumed)
await receiver.CompleteMessageAsync(receivedMessage);