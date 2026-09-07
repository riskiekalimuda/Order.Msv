using MassTransit;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Parameters;
using Microsoft.EntityFrameworkCore;
using Order.Msv.Consumers;
using Order.Msv.Models;
using Order.Msv.Profiles;
using Order.Msv.Services;
// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderMsvDBConnection")));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.AddScoped<OrderService>();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMQParameter>()?? new RabbitMQParameter();
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrderMsvDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
        o.QueryDelay = TimeSpan.FromSeconds(10);
    });

    x.AddConsumersFromNamespaceContaining<OrderCreatedResultConsumer>();    

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

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.MapControllers();


app.Run();

