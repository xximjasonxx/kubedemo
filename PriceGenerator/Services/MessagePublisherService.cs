
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace PriceGenerator.Services
{
    public class MessagePublisherService
    {
        private readonly string _rabbitMqUsername;
        private readonly string _rabbitMqPassword;
        private readonly string _rabbitMqHostname;
        private readonly string _rabbitMqPort;
        private readonly string _rabbitMqExchangeName;

        public MessagePublisherService(IConfiguration configuration)
        {
            var authSection = configuration.GetSection("RabbitMqAuth");
            _rabbitMqUsername = authSection.GetValue<string>("username");
            _rabbitMqPassword = authSection.GetValue<string>("password");

            var connectSection = configuration.GetSection("RabbitMqConnect");
            _rabbitMqHostname = connectSection.GetValue<string>("hostname");
            _rabbitMqPort = connectSection.GetValue<string>("port");

            _rabbitMqExchangeName = configuration.GetValue<string>("RabbitMqExchangeName");
        }

        public Task PublishPriceChanges(IDictionary<string, decimal> stockPriceData)
        {
            using (var rabbitConnection = BuildConnection())
            {
                using (var channel = rabbitConnection.CreateModel())
                {
                    channel.ExchangeDeclare(_rabbitMqExchangeName,
                        type: ExchangeType.Direct,
                        durable: false,
                        autoDelete: false);

                    Parallel.ForEach(stockPriceData, (stockPrice) =>
                    {
                        PublishPriceChange(stockPrice.Key, stockPrice.Value, channel);
                    });
                }
            }

            return Task.CompletedTask;
        }

        void PublishPriceChange(string symbol, decimal value, IModel channel)
        {
            var payload = new JObject(
                new JProperty("symbol", symbol),
                new JProperty("newPrice", value.ToString())
            );
            
            var messagePayload = payload.ToString();
            var message = Encoding.UTF8.GetBytes(messagePayload);
            channel.BasicPublish(exchange: _rabbitMqExchangeName,
                routingKey: string.Empty,
                basicProperties: null,
                body: message);

            Console.WriteLine("Successfully wrote to the exchange");
        }

        IConnection BuildConnection()
        {
            var uriString = $"amqp://{_rabbitMqUsername}:{_rabbitMqPassword}@{_rabbitMqHostname}:{_rabbitMqPort}";
            Console.WriteLine(uriString);

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(uriString);

            return factory.CreateConnection();
        }
    }
}