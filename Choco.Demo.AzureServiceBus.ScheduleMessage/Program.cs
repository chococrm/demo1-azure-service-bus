using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus;

const string messageContent = "Hello, World!";

// 1.) create service bus client
var serviceBusClient = new ServiceBusClient(Config.ConnectionString);

// 2.) create sender and send message, with 1 minute scheduled
var sender = serviceBusClient.CreateSender(Config.QueueName);
var message = new ServiceBusMessage(messageContent);

await sender.ScheduleMessageAsync(message, DateTimeOffset.UtcNow.AddMinutes(1));
Console.WriteLine($"Scheduled message to {message.ScheduledEnqueueTime.ToString()}");

// 3.) create receiver
var receiver = serviceBusClient.CreateReceiver(Config.QueueName);

// wait until message is received
var flag = false;
while (!flag)
{
    // receive message
    var receivedMessage = await receiver.ReceiveMessageAsync();
    // no message available
    if (receivedMessage == null)
    {
        Thread.Sleep(5000);
        continue;
    }
    
    var receivedMessageContent = receivedMessage.Body.ToString();
    Console.WriteLine(receivedMessageContent);
    
    // complete receiving (consumed)
    await receiver.CompleteMessageAsync(receivedMessage);
    
    flag = true;
}
