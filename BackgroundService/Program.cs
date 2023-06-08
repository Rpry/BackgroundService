using BackgroundService.Consumers;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace BackgroundService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x => {
                        x.AddConsumer<EventConsumer>();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            Configure(cfg);
                            RegisterEndPoints(cfg);
                        });
                    });
                    services.AddHostedService<MasstransitService>();
                });
        
        /// <summary>
        /// КОнфигурирование
        /// </summary>
        /// <param name="configurator"></param>
        private static void Configure(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host("hawk.rmq.cloudamqp.com",
                "iatvfquz",
                h =>
                {
                    h.Username("iatvfquz");
                    h.Password("G68bk0zxzH0ncOvMlmfyYapLaCqwjiRi");
                });
        }
        
        /// <summary>
        /// регистрация эндпоинтов
        /// </summary>
        /// <param name="configurator"></param>
        private static void RegisterEndPoints(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint($"masstransit_event_queue_1", e =>
            {
                e.Consumer<EventConsumer>();
                e.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                });
            });

        }
    }
}