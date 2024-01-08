using BackgroundService.Consumers;
using BackgroundService.Settings;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using WebApi.Settings;

namespace BackgroundService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<EventConsumer>();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            ConfigureRmq(cfg, configuration);
                            RegisterEndPoints(cfg);
                        });
                    });
                    services.AddHostedService<MasstransitService>();
                });
        }

        /// <summary>
        /// Конфигурирование RMQ.
        /// </summary>
        /// <param name="configurator"> Конфигуратор RMQ. </param>
        /// <param name="configuration"> Конфигурация приложения. </param>
        private static void ConfigureRmq(IRabbitMqBusFactoryConfigurator configurator, IConfiguration configuration)
        {
            var rmqSettings = configuration.Get<ApplicationSettings>().RmqSettings;
            configurator.Host(rmqSettings.Host,
                rmqSettings.VHost,
                h =>
                {
                    h.Username(rmqSettings.Login);
                    h.Password(rmqSettings.Password);
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