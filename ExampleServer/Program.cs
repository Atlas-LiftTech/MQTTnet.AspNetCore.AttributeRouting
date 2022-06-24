using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;

namespace Example
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
                webBuilder.UseKestrel(
                    o =>
                    {
                        o.ListenAnyIP(50483, l => l.UseMqtt()); // MQTT pipeline
                        o.ListenAnyIP(50482); // Default HTTP pipeline
                    });
                webBuilder.ConfigureLogging(opts => opts.AddConsole());
                webBuilder.UseStartup<Startup>();
            });
    }
}
