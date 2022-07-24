using SlimMessageBus;

namespace HBDStack.SlimMessageBus.AzureBus.Tests.Data;

public class ConsumerHandler: IConsumer<Message>
{
    public Task OnHandle(Message message, string path)
    {
        Console.WriteLine($"Handled message: {message.Body}");
        return Task.CompletedTask;
    }
}