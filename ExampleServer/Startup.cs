using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.AttributeRouting;
using MQTTnet.Server;

namespace Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure AspNetCore controllers
            services.AddControllers();

            services.AddSingleton<MqttServer>();

            // Identify and build routes for the current assembly
            services.AddMqttControllers();
            services
                .AddHostedMqttServerWithServices(s =>
                {
                    // Optionally set server options here
                    s.WithoutDefaultEndpoint();
                })
                .AddMqttConnectionHandler()
                .AddConnections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Root endpoint for MQTT - attribute routing picks up after this URL
                endpoints.MapConnectionHandler<MqttConnectionHandler>(
                    "/mqtt",
                    opts => opts.WebSockets.SubProtocolSelector = protocolList => protocolList.FirstOrDefault() ?? string.Empty);
            });

            app.UseMqttServer(server =>
            {
                // Enable Attribute routing
                server.WithAttributeRouting(app.ApplicationServices, true);
            });
        }
    }
}