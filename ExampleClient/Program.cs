using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using System;
using System.Collections.Generic;

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
            await mqttClient.SubscribeAsync(new List<MqttTopicFilter> { new MqttTopicFilterBuilder().WithTopic("MqttWeatherForecast/90210/temperature").Build() });

            mqttClient.ConnectedAsync += (e) =>
            {
                Console.WriteLine($"Connection Result: {e.ConnectResult.ResultCode}");
                return System.Threading.Tasks.Task.CompletedTask;
            };

            mqttClient.ConnectingFailedAsync += (e) =>
            {
                Console.WriteLine($"Connection Failed: {e.Exception}");
                return System.Threading.Tasks.Task.CompletedTask;
            };

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine($"Message from {e.ClientId}: {e.ApplicationMessage.Payload.Length} bytes.");
                return System.Threading.Tasks.Task.CompletedTask;
            };

            await mqttClient.StartAsync(options);

            // Publish a message on a well known topic
            await mqttClient.EnqueueAsync(new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(msg =>
            {
                msg.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                msg.WithPayload(BitConverter.GetBytes(98.6d));
                msg.WithTopic("MqttWeatherForecast/90210/temperature");
            }).Build());

            // Publish a message on a topic the server doesn't explicitly handle
            await mqttClient.EnqueueAsync(new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(msg =>
            {
                msg.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                msg.WithPayload(BitConverter.GetBytes(100d));
                msg.WithTopic("asdfsdfsadfasdf");
            }).Build());

            // StartAsync returns immediately, as it starts a new thread using Task.Run, and so the calling thread needs
            // to wait.
            Console.ReadLine();
        }
    }
}