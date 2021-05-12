using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;

namespace ExampleClient
{
    internal class Program
    {
        private static async System.Threading.Tasks.Task Main(string[] args)
        {
            var rnd = new Random();
            // Setup and start a managed MQTT client.
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId($"Client{rnd.Next(0, 1000)}")
                    .WithWebSocketServer("localhost:50482/mqtt")
                    .Build())
                .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("MqttWeatherForecast/90210/temperature").Build());

            mqttClient.UseConnectedHandler(e =>
            {
                Console.WriteLine($"Connection Result: {e.AuthenticateResult.ResultCode}");
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"Message from {e.ClientId}: {e.ApplicationMessage.Payload.Length} bytes.");
            });

            await mqttClient.StartAsync(options);

            // Publish a message on a well known topic
            await mqttClient.PublishAsync(new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(msg =>
             {
                 msg.WithAtLeastOnceQoS();
                 msg.WithPayload(BitConverter.GetBytes(98.6d));
                 msg.WithTopic("MqttWeatherForecast/90210/temperature");
             }).Build());

            // Publish a message on a topic the server doesn't explicitly handle
            await mqttClient.PublishAsync(new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(msg =>
             {
                 msg.WithAtLeastOnceQoS();
                 msg.WithPayload(BitConverter.GetBytes(100d));
                 msg.WithTopic("asdfsdfsadfasdf");
             }).Build());

            // StartAsync returns immediately, as it starts a new thread using Task.Run, and so the calling thread needs
            // to wait.
            Console.ReadLine();
        }
    }
}