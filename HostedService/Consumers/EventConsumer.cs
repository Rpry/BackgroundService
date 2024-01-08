using CommonNamespace;
using MassTransit;

namespace BackgroundService.Consumers
{
    class EventConsumer : IConsumer<MessageDto>
    {
        public async Task Consume(ConsumeContext<MessageDto> context)
        {
            //throw new ArgumentException("some error");
            Console.WriteLine("Value: {0}", context.Message.Content);
        }
    }
}