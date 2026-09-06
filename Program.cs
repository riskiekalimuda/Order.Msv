using MassTransit;
using MessageMQCommon.Parameters;
using Microsoft.EntityFrameworkCore;
using Order.Msv.Models;
using Order.Msv.Profiles;
// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderMsvDBConnection")));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMQParameter>()?? new RabbitMQParameter();
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrderMsvDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
        o.QueryDelay = TimeSpan.FromSeconds(10);
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.MapControllers();


app.Run();

