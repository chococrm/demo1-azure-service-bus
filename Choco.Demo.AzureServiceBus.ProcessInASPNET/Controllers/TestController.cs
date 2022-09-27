using Azure.Messaging.ServiceBus;
using Choco.Demo.AzureServiceBus.ProcessInASPNET.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Choco.Demo.AzureServiceBus.ProcessInASPNET.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly ServiceBusClient _serviceBusClient;

    public TestController(ILogger<TestController> logger, ServiceBusClient serviceBusClient)
    {
        _logger           = logger;
        _serviceBusClient = serviceBusClient;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] TestSendMessageDto body)
    {
        // create sender
        var sender = _serviceBusClient.CreateSender(Config.QueueName);

        // send message
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(body));
        await sender.SendMessageAsync(message);

        // return result
        return new OkObjectResult(new
        {
            message.MessageId,
        });
    }
}