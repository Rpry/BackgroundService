using CommonNamespace;
using MassTransit;

namespace BackgroundService.Consumers
{
    class EventConsumer : IConsumer<MessageDto>
    {
        public async Task Consume(ConsumeContext<MessageDto> context)
        {
            //throw new ArgumentException("some error");
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine("Value: {0}", context.Message.Content);
        }
    }
}