using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Order.Msv.Extensions
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddOrderTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            // Mengambil nama service secara dinamis atau hardcode khusus untuk Order Service
            var serviceName = "OrderService";

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options => options.RecordException = true) // Menangkap incoming HTTP request dari YARP
                        .AddHttpClientInstrumentation(options => options.RecordException = true) // Menangkap jika Order memanggil Purchase via HTTP
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317");
                        });
                });

            return services;
        }
    }
}
