using MassTransit;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Parameters;
using Microsoft.EntityFrameworkCore;
using Order.Msv.Consumers;
using Order.Msv.Extensions;
using Order.Msv.Models;
using Order.Msv.Profiles;
using Order.Msv.Services;
// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderTelemetry(builder.Configuration);

builder.Services.AddDbContext<OrderMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderMsvDBConnection")));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.AddScoped<OrderService>();

builder.Services.AddCustomMassTransit(builder.Configuration);

var app = builder.Build();
app.UseRouting();
app.MapControllers();


app.Run();

