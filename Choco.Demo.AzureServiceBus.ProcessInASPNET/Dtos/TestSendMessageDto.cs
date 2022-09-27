namespace Choco.Demo.AzureServiceBus.ProcessInASPNET.Dtos;

public class TestSendMessageDto
{
    public string Message { get; set; } = string.Empty;
    public bool PleaseDead { get; set; } = false;
}