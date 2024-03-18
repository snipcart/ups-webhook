using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Snipcart.UPS_Webhook.Configurations;
using Snipcart.UPS_Webhook.Services;

namespace Snipcart.UPS_Webhook
{
    public static class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration(builder =>
                {
                    builder
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(s =>
                {
                    s.AddHttpClient();
                    s.AddHttpClient<IUpsService, UpsService>();
                
                    s.AddOptions<UpsConfiguration>()
                        .Configure<IConfiguration>((settings, configuration) =>
                        {
                            configuration.GetSection("Ups").Bind(settings);
                        });
                    s.AddOptions<BusinessAddress>()
                        .Configure<IConfiguration>((settings, configuration) =>
                        {
                            configuration.GetSection("BusinessAddress").Bind(settings);
                        });
                })
                .Build();

            host.Run();
        }
    }
}

