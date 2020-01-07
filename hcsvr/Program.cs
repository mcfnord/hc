using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hcsvr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*
                    webBuilder.UseKestrel(
                        options =>
                        {
                            options.Listen(System.Net.IPAddress.Any, 80);
                      //      options.Listen(System.Net.IPAddress.Parse("52.35.121.62"), 80);
                        }
                        );
                        */

                    //    webBuilder.UseIISIntegration();
                    /*
                    webBuilder.UseKestrel((context, serverOptions) =>
                    {
                        serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
                            .Endpoint("HTTPS", listenOptions =>
                            {
                                listenOptions.HttpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                            });
                    });
                    */

                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(
                        options =>
                        {
                            options.Listen(System.Net.IPAddress.Any, 80);
                        }
                        );
                });
    }
}
