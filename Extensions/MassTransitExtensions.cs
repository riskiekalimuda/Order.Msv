using MassTransit;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Parameters;
using Order.Msv.Consumers;
using Order.Msv.Models;

namespace Order.Msv.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMqSettings").Get<RabbitMQParameter>() ?? new RabbitMQParameter();
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<OrderMsvDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(10);
                });

                x.AddConsumersFromNamespaceContaining<OrderCreatedResultConsumer>();
                x.AddConsumersFromNamespaceContaining<UpdateOrderResultConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.ReceiveEndpoint(QueueNames.OrderQueue.AddOrderResultQueue, e =>
                    {
                        e.Durable = true;
                        e.UseMessageRetry(r => r.Interval(20, 10));
                        e.ConfigureConsumer<OrderCreatedResultConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueNames.OrderQueue.UpdateOrderResultQueue, e =>
                    {
                        e.Durable = true;
                        e.UseMessageRetry(r => r.Interval(20, 10));
                        e.ConfigureConsumer<UpdateOrderResultConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
