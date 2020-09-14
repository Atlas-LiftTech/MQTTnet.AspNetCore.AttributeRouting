using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore.AttributeRouting;
using System;
using System.Threading.Tasks;

namespace Example.MqttControllers
{
    [MqttController]
    [MqttRoute("[controller]")]
    public class MqttWeatherForecastController : MqttBaseController
    {
        private readonly ILogger<MqttWeatherForecastController> _logger;

        // Controllers have full support for dependency injection just like AspNetCore controllers
        public MqttWeatherForecastController(ILogger<MqttWeatherForecastController> logger)
        {
            _logger = logger;
        }

        // Supports template routing with typed constraints
        [MqttRoute("{zipCode:int}/temperature")]
        public Task WeatherReport(int zipCode)
        {
            // We have access to the MqttContext
            if (zipCode != 90210) { MqttContext.CloseConnection = true; }

            // We have access to the raw message
            var temperature = BitConverter.ToDouble(Message.Payload);

            _logger.LogInformation($"It's {temperature} degrees in Hollywood");

            // Example validation
            if (temperature <= 0 || temperature >= 130)
            {
                return BadMessage();
            }

            return Ok();
        }
    }
}