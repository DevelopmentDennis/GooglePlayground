using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GooglePlaces.Services;
using GooglePlaces.Models.Options;
using Microsoft.Extensions.Configuration;

namespace Example.Function
{
    public class Program
    {

        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureLogging(logging =>
                {
                    logging.Services.AddLogging();
                })
                .ConfigureServices(s =>
                {
                    s.AddScoped<IEmailService, EmailService>();
                    s.AddHttpClient<IGoogleMapsService, GoogleMapsService>();

                    s.AddOptions<GoogleMapsPlacesApiOptions>().Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.GetSection("GoogleRequestOptions").Bind(settings);
                    });

                    s.AddOptions<EmailOptions>().Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.GetSection("EmailOptions").Bind(settings);
                    });
                })
                .Build();

            host.Run();
        }
    }
}